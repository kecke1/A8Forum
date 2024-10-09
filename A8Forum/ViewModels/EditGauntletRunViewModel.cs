using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class EditGauntletRunViewModel
{
    [Display(Name = "Id")]
    public string? GauntletRunId { get; set; }

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

    [Display(Name = "Vehicle 1")]
    public required string Vehicle1Id { get; set; }

    [Display(Name = "Vehicle 2")]
    public required string Vehicle2Id { get; set; }

    [Display(Name = "Vehicle 3")]
    public required string Vehicle3Id { get; set; }

    [Display(Name = "Vehicle 4")]
    public string? Vehicle4Id { get; set; }

    [Display(Name = "Vehicle 5")]
    public string? Vehicle5Id { get; set; }

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

    [Display(Name = "✅")]
    public bool LapTimeVerified { get; set; } = false;
}