using Shared.Dto;
using System.ComponentModel.DataAnnotations;
using Shared.Services;

namespace A8Forum.ViewModels
{
    public class GauntletLeaderboardFilterInput : IValidatableObject
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

        public bool IncludeUnverifiedRuns { get; set; } = true;

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

    public class GauntletLeaderboardViewModel
    {
        public GauntletLeaderboardFilterInput Filter { get; set; } = new();

        // TOTAL tab
        public IEnumerable<GauntletService.GauntletLeaderboardResultDto> TotalResults { get; set; } = [];
        public int TotalCount { get; set; }

        // BY TRACK (groups with Name + Rows)
        public IEnumerable<GroupedGauntletLeaderboardRowsDto> ByTrackGroups { get; set; } = [];
        public int ByTrackCount { get; set; }

        // BEST LAP TIMES (groups with Name + Rows)
        public IEnumerable<GroupedGauntletLeaderboardRowsDto> BestLapGroups { get; set; } = [];
        public int BestLapCount { get; set; }
    }
}
