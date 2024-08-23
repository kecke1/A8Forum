using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class UsersViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public bool Locked { get; set; }

    [Display(Name = "Forum Member")]
    public string MemberName { get; set; }

    public IEnumerable<string> Roles { get; set; }
}