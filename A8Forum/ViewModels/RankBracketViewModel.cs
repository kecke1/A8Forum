using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace A8Forum.ViewModels;

public class RankBracketViewModel
{
    [Display(Name = "Id")]
    public string? RankBracketId { get; set; }

    [Required]
    [Display(Name = "Min Rank")]
    public int MinRank { get; set; }

    [Required]
    [Display(Name = "Max Rank")]
    public int MaxRank { get; set; }

    [Required]
    [Display(Name = "Class")]
    public ClassEnum Class { get; set; }
}