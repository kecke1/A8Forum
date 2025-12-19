using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class EditSprintRunViewModel
{
    [Display(Name = "Id")]
    public string? SprintRunId { get; set; }

    public int Time { get; set; }

    [Display(Name = "Lap Time")]
    public required string TimeString { get; set; }

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Idate { get; set; } = DateTime.Now;

    public bool Deleted { get; set; } = false;

    [Display(Name = "Track")]
    public required string TrackId { get; set; }

    [Display(Name = "Vehicle")]
    public required string VehicleId { get; set; }

    [Display(Name = "Forum Member")]
    public string? MemberId { get; set; }

    [Display(Name = "Forum Post")]
    public required string PostUrl { get; set; }

    [Display(Name = "Run Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? RunDate { get; set; }

    [Display(Name = "Video Link")]
    public string? MediaLink { get; set; }
    public bool Save { get; set; } = true;

    [Display(Name = "VIP Level")]
    public int? VipLevel { get; set; }
    [Display(Name = "Shortcut")]
    public bool Shortcut { get; set; }
    [Display(Name = "Glitch")]
    public bool Glitch { get; set; }
}