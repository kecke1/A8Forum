using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class EditUserViewModel
{
    [Display(Name = "User Id")]
    public required string UserId { get; set; }

    [Display(Name = "User Name")]
    public required string UserName { get; set; }

    [Display(Name = "Forum Member")]
    public string MemberId { get; set; }

    [Display(Name = "Locked out")]
    public bool LockedOut { get; set; } = false;
}