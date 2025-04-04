using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class SprintTrackReferencePointViewModel
{
    [Display(Name = "Id")]
    public string? SprintTrackReferencePointId { get; set; }

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    public TrackViewModel Track { get; set; }
}
