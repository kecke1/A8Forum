using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class DeleteUserViewModel
{
    [Display(Name = "User Name")]
    public string UserName { get; set; }

    [Display(Name = "User Id")]
    public string UserId { get; set; }
}