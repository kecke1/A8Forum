using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class CreateGauntletRunFromTemplateViewModel
{
    [Display(Name = "Forum Post")]
    public string PostUrl { get; set; }

    [Display(Name = "Forum Member")]
    public string? MemberId { get; set; }

    [Display(Name = "Template Text")]
    public string TemplateText { get; set; }
}