using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class MemberViewModel
{
    public string? MemberId { get; set; }

    [Display(Name = "Forum Id")]
    public string? MemberName { get; set; }

    [Display(Name = "Display Name")]
    public string? MemberDisplayName { get; set; }

    public bool Guest { get; set; } = false;

    [Display(Name = "VIP Level")]
    public int? VipLevel { get; set; }

    [Display(Name = "Racing names")]
    public string? RacingNames { get; set; }
    [Display(Name = "Former Racing names")]
    public string? FormerRacingNames { get; set; }

    [Display(Name = "Deleted")]
    public bool Deleted { get; set; }
    [Display(Name = "Hidden")]
    public bool Hidden { get; set; }
}