using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class CreateSprintRunFromTemplateViewModel
{
    [Display(Name = "Forum Post")]
    public string PostUrl { get; set; }

    [Display(Name = "Template Text")]
    public string TemplateText { get; set; }
}