using Microsoft.AspNetCore.Identity;

namespace A8Forum.Areas.Identity.Data;

// Add profile data for application users by adding properties to the A8ForumazurewebsitesnetUser class
public class A8ForumazurewebsitesnetUser : IdentityUser
{
    public string? MemberId { get; set; }
}