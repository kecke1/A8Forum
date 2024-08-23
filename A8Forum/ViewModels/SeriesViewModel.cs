using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class SeriesViewModel
{
    public string? SeriesId { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }

    [Display(Name = "BB Code")]
    public string Leaderboard { get; set; } = "";

    [Display(Name = "Leaderboard")]
    public string LeaderboardHtml { get; set; } = "";
}