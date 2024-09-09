using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class ChangePasswordViewModel
{
    public required string UserId { get; set; }

    [Display(Name = "User Name")]
    public required string UserName { get; set; }

    [Display(Name = "Password")]
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [DataType(DataType.Password)]
    public required string ConfirmPassword { get; set; }
}