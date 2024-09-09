using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class DeleteUserViewModel
{
    [Display(Name = "User Name")]
    public required string UserName { get; set; }

    [Display(Name = "User Id")]
    public required string UserId { get; set; }
}