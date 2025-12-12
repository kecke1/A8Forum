using A8Forum.Areas.Identity.Data;
using A8Forum.Data;
using A8Forum.Enums;
using AspNetCore.Identity.CosmosDb.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dto;
using Shared.Models;
using Shared.Options;
using Shared.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<A8Options>(builder.Configuration);
var options = builder.Configuration.Get<A8Options>();
bool.TryParse(options.SetupCosmosDb, out var setup);

#if DEBUG

builder.Services.AddDbContext<A8ForumazurewebsitesnetContex>(o =>
    o.UseSqlServer(options.CosmosConnection));

builder.Services.AddIdentity<A8ForumazurewebsitesnetUser, IdentityRole>(o => o.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<A8ForumazurewebsitesnetContex>()
    .AddDefaultUI(); // Use this if Identity Scaffolding is in use
//.AddDefaultTokenProviders();

#else


// If this is set, the Cosmos identity provider will:
// 1. Create the database if it does not already exist.
// 2. Create the required containers if they do not already exist.
// IMPORTANT: Remove this setting if after first run. It will improve startup performance.

// If the following is set, it will create the Cosmos database and
//  required containers.
if (setup)
{
    var builder1 = new DbContextOptionsBuilder<A8ForumazurewebsitesnetContex>();
    builder1.UseCosmos(options.CosmosConnection, options.CosmosDb);

    using (var dbContext = new A8ForumazurewebsitesnetContex(builder1.Options))
    {
        dbContext.Database.EnsureCreated();
    }
}

builder.Services.AddDbContext<A8ForumazurewebsitesnetContex>(o =>
    o.UseCosmos(options.CosmosConnection, options.CosmosDb));

builder.Services.AddCosmosIdentity<A8ForumazurewebsitesnetContex, A8ForumazurewebsitesnetUser, IdentityRole, string>(
        options =>
        {
            options.SignIn.RequireConfirmedAccount = true; // Always a good idea :)
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = false;
        }, new TimeSpan(30, 0, 0, 0), true
    )
    .AddDefaultUI(); // Use this if Identity Scaffolding is in use
//.AddDefaultTokenProviders();

#endif

// Add services to the container.
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminRole", policy => policy.RequireRole(nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("ForumChallengeUserRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.ForumChallengeUser),
            nameof(IdentityRoleEnum.ForumChallengeAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GauntletUserRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.GauntletUser), nameof(IdentityRoleEnum.GauntletAdmin),
            nameof(IdentityRoleEnum.Admin)));
    x.AddPolicy("ForumChallengeAdminRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.ForumChallengeAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GauntletAdminRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.GauntletAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GiftLinkRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("SprintUserRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.SprintUser), nameof(IdentityRoleEnum.SprintAdmin),
            nameof(IdentityRoleEnum.Admin)));
    x.AddPolicy("SprintAdminRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.SprintAdmin), nameof(IdentityRoleEnum.Admin)));

});

#if DEBUG
builder.Services.AddInMemoryCosmosRepository();


#else

builder.Services.AddCosmosRepository(
    x =>
    {
        x.CosmosConnectionString = options.CosmosConnection;
        x.DatabaseId = options.CosmosDb;
        x.ContainerPerItemType = true;
    });
#endif
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<IGauntletService, GauntletService>();
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<IForumChallengeService, ForumChallengeService>();
builder.Services.AddScoped<IGiftLinkService, GiftLinkService>();
builder.Services.AddScoped<IDataManagementService, DataManagementService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (setup)
{
    using var scope = app.Services.CreateScope();
    //initializing custom roles
    var userManager =
        (UserManager<A8ForumazurewebsitesnetUser>)scope.ServiceProvider.GetService(
            typeof(UserManager<A8ForumazurewebsitesnetUser>));
    var RoleManager =
        (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));

    foreach (var roleName in Enum.GetNames<IdentityRoleEnum>())
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
            //create the roles and seed them to the database
            await RoleManager.CreateAsync(new IdentityRole(roleName));
    }

    //Create a super user who will maintain the web app
    if (!string.IsNullOrEmpty(options.AdminUser))
    {
        var poweruser = new A8ForumazurewebsitesnetUser
        {
            UserName = options.AdminUser,
            Email = $"{options.AdminUser}@A8Forum.azurewebsites.net",
            EmailConfirmed = true
        };

        //Ensure you have these values in your appsettings.json file
        var userPWD = options.AdminPassword;
        var _user = await userManager.FindByNameAsync(poweruser.UserName);

        if (_user == null)
        {
            var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
            if (createPowerUser.Succeeded)
                //here we tie the new user to the role
                await userManager.AddToRoleAsync(poweruser, nameof(IdentityRoleEnum.Admin));
        }
    }

#if DEBUG
    var dataManger = (IDataManagementService)scope.ServiceProvider.GetService(typeof(IDataManagementService));
    var content = System.IO.File.ReadAllText(options.ImportFile);
    var jsonopts = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var data = JsonSerializer.Deserialize<ExportDataDto>(content, jsonopts);

    dataManger.ImportData(data);
#endif
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    "default",
    "{controller=GauntletLeaderboard}/{action=Index}/{id?}");

app.Run();