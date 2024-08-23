using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class TrackViewModel
{
    [Display(Name = "Id")]
    public string? TrackId { get; set; }

    [Display(Name = "Track Name")]
    public string? TrackName { get; set; }
}