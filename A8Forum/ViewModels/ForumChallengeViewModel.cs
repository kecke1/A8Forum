using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class ForumChallengeViewModel
{
    [Display(Name = "Id")]
    public string? ForumChallengeId { get; set; }

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

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime Idate { get; set; } = DateTime.Now;

    [Display(Name = "Post URL")]
    public string? Post { get; set; } = "";

    [Display(Name = "Custom Title")]
    public string? CustomTitle { get; set; } = "";

    [Display(Name = "LeaderboardSimple")]
    public string LeaderboardSimple { get; set; } = "";

    [Display(Name = "LeaderboardSimpleBB")]
    public string LeaderboardSimpleBB { get; set; } = "";

    [Display(Name = "LeaderboardFullHtml")]
    public string LeaderboardFullHtml { get; set; } = "";

    [Display(Name = "LeaderboardFullBB")]
    public string LeaderboardFullBB { get; set; } = "";

    [Display(Name = "LeaderboardHtml")]
    public string LeaderboardHtml { get; set; } = "";

    [Display(Name = "LeaderboardHtml")]
    public string LeaderboardBB { get; set; } = "";

    public bool Deleted { get; set; }

    [Display(Name = "Max Rank")]
    public string MaxRank { get; set; } = "";

    [Display(Name = "Track")]
    public TrackViewModel? Track { get; set; } = new TrackViewModel();

    [Display(Name = "Season")]
    public SeasonViewModel? Season { get; set; } = new SeasonViewModel();
}