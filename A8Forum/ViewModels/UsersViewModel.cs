using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class UsersViewModel
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public bool Locked { get; set; }

    [Display(Name = "Forum Member")]
    public string? MemberName { get; set; }

    public required IEnumerable<string> Roles { get; set; }
}