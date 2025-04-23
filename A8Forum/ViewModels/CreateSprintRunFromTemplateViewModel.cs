using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class CreateSprintRunFromTemplateViewModel
{
    [Display(Name = "Forum Post")]
    public string PostUrl { get; set; }

    [Display(Name = "Forum Member")]
    public string? MemberId { get; set; }

    [Display(Name = "Forum post Text")]
    public string TemplateText { get; set; }
}