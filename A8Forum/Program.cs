using A8Forum.Areas.Identity.Data;
using A8Forum.Data;
using A8Forum.Enums;
using AspNetCore.Identity.CosmosDb.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Options;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<A8Options>(builder.Configuration);
var options = builder.Configuration.Get<A8Options>();

// If this is set, the Cosmos identity provider will:
// 1. Create the database if it does not already exist.
// 2. Create the required containers if they do not already exist.
// IMPORTANT: Remove this setting if after first run. It will improve startup performance.

// If the following is set, it will create the Cosmos database and
//  required containers.
if (bool.TryParse(options.SetupCosmosDb, out var setup) && setup)
{
    var builder1 = new DbContextOptionsBuilder<A8ForumazurewebsitesnetContex>();
    builder1.UseCosmos(options.CosmosConnection, options.CosmosDb);

    using (var dbContext = new A8ForumazurewebsitesnetContex(builder1.Options))
    {
        dbContext.Database.EnsureCreated();
    }
}

builder.Services.AddDbContext<A8ForumazurewebsitesnetContex>(o =>
    o.UseCosmos(connectionString: options.CosmosConnection, databaseName: options.CosmosDb));

builder.Services.AddCosmosIdentity<A8ForumazurewebsitesnetContex, A8ForumazurewebsitesnetUser, IdentityRole, string>(
      options =>
      {
          options.SignIn.RequireConfirmedAccount = true; // Always a good idea :)
          options.SignIn.RequireConfirmedEmail = false;
          options.User.RequireUniqueEmail = false;
      }
    )
    .AddDefaultUI(); // Use this if Identity Scaffolding is in use
                     //.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminRole", policy => policy.RequireRole(nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("ForumChallengeUserRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.ForumChallengeUser), nameof(IdentityRoleEnum.ForumChallengeAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GauntletUserRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.GauntletUser), nameof(IdentityRoleEnum.GauntletAdmin), nameof(IdentityRoleEnum.Admin)));
    x.AddPolicy("ForumChallengeAdminRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.ForumChallengeAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GauntletAdminRole",
        policy => policy.RequireRole(nameof(IdentityRoleEnum.GauntletAdmin), nameof(IdentityRoleEnum.Admin)));

    x.AddPolicy("GiftLinkRole",
    policy => policy.RequireRole(nameof(IdentityRoleEnum.Admin)));
});

builder.Services.AddCosmosRepository(
x =>
{
    x.CosmosConnectionString = options.CosmosConnection;
    x.DatabaseId = options.CosmosDb;
    x.ContainerPerItemType = true;
});

builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<IGauntletService, GauntletService>();
builder.Services.AddScoped<IForumChallengeService, ForumChallengeService>();
builder.Services.AddScoped<IGiftLinkService, GiftLinkService>();
builder.Services.AddScoped<IDataManagementService, DataManagementService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (setup)
{
    using (var scope = app.Services.CreateScope())
    {
        //initializing custom roles
        var UserManager = (UserManager<A8ForumazurewebsitesnetUser>)scope.ServiceProvider.GetService(typeof(UserManager<A8ForumazurewebsitesnetUser>));
        var RoleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));

        foreach (var roleName in Enum.GetNames<IdentityRoleEnum>())
        {
            var roleExist = await RoleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                //create the roles and seed them to the database
                await RoleManager.CreateAsync(new IdentityRole(roleName));
            }
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
            string userPWD = options.AdminPassword;
            var _user = await UserManager.FindByNameAsync(poweruser.UserName);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, nameof(IdentityRoleEnum.Admin));
                }
            }
        }
    }
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
    name: "default",
    pattern: "{controller=GauntletLeaderboard}/{action=Index}/{id?}");

app.Run();