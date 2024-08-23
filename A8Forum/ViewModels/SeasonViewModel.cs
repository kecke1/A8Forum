using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class SeasonViewModel
{
    [Display(Name = "Id")]
    public string? SeasonId { get; set; }

    [Display(Name = "Season Name")]
    public string? SeasonName { get; set; } = "";
}