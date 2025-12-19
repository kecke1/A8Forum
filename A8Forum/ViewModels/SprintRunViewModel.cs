using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class SprintRunViewModel
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
    public TrackViewModel? Track { get; set; } = new();

    [Display(Name = "Vehicle")]
    public VehicleViewModel? Vehicle { get; set; } = new();

    [Display(Name = "Forum Member")]
    public MemberViewModel? Member { get; set; } = new();

    [Display(Name = "Forum Post")]
    public required string PostUrl { get; set; }

    [Display(Name = "Run Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? RunDate { get; set; }

    [Display(Name = "Video Link")]
    public string? MediaLink { get; set; }

    [Display(Name = "VIP Level")]
    public int? VipLevel { get; set; }

    [Display(Name = "Shortcut")]
    public bool Shortcut { get; set; }
    [Display(Name = "Glitch")]
    public bool Glitch { get; set; }
}