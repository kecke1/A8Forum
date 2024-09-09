using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace A8Forum.ViewModels;

public class CareerRaceViewModel
{
    [Display(Name = "Id")]
    public string? CareerRaceId { get; set; }

    public CareerRaceTypeEnum CareerRaceType { get; set; } = CareerRaceTypeEnum.Classic;

    [Required]
    public required TrackViewModel Track { get; set; }

    public ClassEnum? Class { get; set; }

    [Required]
    public required SeasonViewModel Season { get; set; }

    [Required]
    public bool Limitations { get; set; } = false;

    public string LimitationsDescription { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
}