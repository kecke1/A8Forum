using A8Forum.Areas.Identity.Data;
using AspNetCore.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace A8Forum.Data;

public class A8ForumazurewebsitesnetContex(DbContextOptions dbContextOptions) : CosmosIdentityDbContext<A8ForumazurewebsitesnetUser, IdentityRole, string>(dbContextOptions)
{
    //  protected override void OnModelCreating(ModelBuilder builder)
    // {
    //    base.OnModelCreating(builder);
    // Customize the ASP.NET Identity model and override the defaults if needed.
    // For example, you can rename the ASP.NET Identity table names and more.
    // Add your customizations after calling base.OnModelCreating(builder);
    // }
}