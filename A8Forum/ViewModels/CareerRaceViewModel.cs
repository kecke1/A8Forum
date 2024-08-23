using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class CareerRaceViewModel
{
    [Display(Name = "Id")]
    public string? CareerRaceId { get; set; }

    public Shared.Enums.CareerRaceTypeEnum CareerRaceType { get; set; } = Shared.Enums.CareerRaceTypeEnum.Classic;

    [Required]
    public TrackViewModel Track { get; set; }

    public Shared.Enums.ClassEnum? Class { get; set; }

    [Required]
    public SeasonViewModel Season { get; set; }

    [Required]
    public bool Limitations { get; set; } = false;

    public string LimitationsDescription { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
}