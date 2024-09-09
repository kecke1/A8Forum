using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class ForumChallengeRunViewModel
{
    [Display(Name = "Id")]
    public string? ForumChallengeRunId { get; set; }

    public int Time { get; set; }

    [Display(Name = "Time")]
    public string? TimeString { get; set; }

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime Idate { get; set; } = DateTime.Now;

    [Display(Name = "Post URL")]
    public string? Post { get; set; } = "";

    public bool Deleted { get; set; } = false;
    public ForumChallengeViewModel? ForumChallenge { get; set; } = new();
    public MemberViewModel? Member { get; set; } = new();
    public VehicleViewModel? Vehicle { get; set; } = new();
}