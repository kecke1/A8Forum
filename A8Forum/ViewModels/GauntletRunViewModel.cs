using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class GauntletRunViewModel
{
    [Display(Name = "Id")]
    public string? GauntletRunId { get; set; }

    public int Time { get; set; }

    [Display(Name = "Time")]
    public required string TimeString { get; set; }

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Idate { get; set; } = DateTime.Now;

    public bool Deleted { get; set; } = false;

    [Display(Name = "Track")]
    public TrackViewModel? Track { get; set; } = new();

    [Display(Name = "Vehicle 1")]
    public VehicleViewModel? Vehicle1 { get; set; } = new();

    [Display(Name = "Vehicle 2")]
    public VehicleViewModel? Vehicle2 { get; set; } = new();

    [Display(Name = "Vehicle 3")]
    public VehicleViewModel? Vehicle3 { get; set; } = new();

    [Display(Name = "Vehicle 4")]
    public VehicleViewModel? Vehicle4 { get; set; } = new();

    [Display(Name = "Vehicle 5")]
    public VehicleViewModel? Vehicle5 { get; set; } = new();

    [Display(Name = "Forunm Member")]
    public MemberViewModel? Member { get; set; } = new();

    [Display(Name = "Post URL")]
    public required string PostUrl { get; set; }

    [Display(Name = "Run Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? RunDate { get; set; }

    [Display(Name = "Media Link")]
    public string? MediaLink { get; set; }

    [Display(Name = "✅")]
    public bool LapTimeVerified { get; set; } = false;
}