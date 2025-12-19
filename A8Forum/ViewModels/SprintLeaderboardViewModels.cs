using Shared.Dto;
using System.ComponentModel.DataAnnotations;
using Shared.Services;

namespace A8Forum.ViewModels
{
    public class LeaderboardFilterInput : IValidatableObject
    {
        [Display(Name = "VIP Level (min)")]
        [Range(0, 15)]
        public int? VipLevelMin { get; set; }

        [Display(Name = "VIP Level (max)")]
        [Range(0, 15)]
        public int? VipLevelMax { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LeaderboardDate { get; set; }
        public bool UseLeaderboardDate { get; set; } = false;

        public bool IncludeFilteredVipRuns { get; set; } = false;

        public bool IncludeRunsWithGlitch { get; set; } = true;
        public bool IncludeRunsWithShortcut { get; set; } = true;

        // Persist which tab is active across GET submits (total | byTrack | bestLaps)
        public string ActiveTab { get; set; } = "total";

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (VipLevelMin.HasValue && VipLevelMax.HasValue && VipLevelMin > VipLevelMax)
            {
                yield return new ValidationResult(
                    "VIP Level (min) cannot be greater than VIP Level (max).",
                    new[] { nameof(VipLevelMin), nameof(VipLevelMax) });
            }
        }
    }

    public class SprintLeaderboardViewModel
    {
        public LeaderboardFilterInput Filter { get; set; } = new();

        // TOTAL tab
        public IEnumerable<SprintService.SprintLeaderboardResultDto> TotalResults { get; set; } = Array.Empty<SprintService.SprintLeaderboardResultDto>();
        public int TotalCount { get; set; }

        // BY TRACK (groups with Name + Rows)
        public IEnumerable<GroupedSprintLeaderboardRowsDto> ByTrackGroups { get; set; } = Array.Empty<GroupedSprintLeaderboardRowsDto>();
        public int ByTrackCount { get; set; }

        // BEST LAP TIMES (groups with Name + Rows)
        public IEnumerable<GroupedSprintLeaderboardRowsDto> BestLapGroups { get; set; } = Array.Empty<GroupedSprintLeaderboardRowsDto>();
        public int BestLapCount { get; set; }
    }
}
