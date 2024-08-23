using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class CreateUserViewModel
{
    [Display(Name = "User Name")]
    public string UserName { get; set; }

    [Display(Name = "Forum Member")]
    public MemberViewModel? Member { get; set; } = new MemberViewModel();

    [Display(Name = "Password")]
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}