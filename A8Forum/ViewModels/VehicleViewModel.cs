using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class VehicleViewModel
{
    public string? VehicleId { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; } = "";
    public string? Keyword { get; set; } = "";
    public string? Url { get; set; }
    public int? MaxRank { get; set; }

    [Display(Name = "Vehicle Name")]
    public string DisplayName => $"{Name} - {MaxRank}";
}