using System.ComponentModel.DataAnnotations;
using Shared.Dto;

namespace A8Forum.ViewModels;

public class LeaderboardViewModel
{
    [Display(Name = "VIP < 12")]
    public bool NonVip { get; set; } = false;

    [Display(Name = "VIP 12 (Handling +1%)")]
    public bool Vip12 { get; set; } = false;

    [Display(Name = "VIP 13 (Nitro speed +4%)")]
    public bool Vip13 { get; set; } = false;

    [Display(Name = "VIP 14 (Top speed +1%)")]
    public bool Vip14 { get; set; } = false;

    [Display(Name = "VIP 15 (Acc +4%)")]
    public bool Vip15 { get; set; } = false;
    public IEnumerable<GauntletLeaderboardRowDto> Leaderboard { get; set; } 
    }