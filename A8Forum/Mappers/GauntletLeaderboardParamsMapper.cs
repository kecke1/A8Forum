using A8Forum.ViewModels;
using Shared.Params;

namespace A8Forum.Mappers
{
    public static class GauntletLeaderboardParamsMapper
    {
        public static GetGauntletLeaderboardRowsParams ToParams(GauntletLeaderboardFilterInput f)
        {
            return new GetGauntletLeaderboardRowsParams
            {
                MinVipLevel = f.VipLevelMin ?? 0,
                MaxVipLevel = f.VipLevelMax ?? 15,
                Date = f.UseLeaderboardDate ? f.LeaderboardDate : null,
                IncludeFilteredOutVipMembers = f.IncludeFilteredVipRuns,
                IncludeUnverified = f.IncludeUnverifiedRuns
            };
        }
    }
}

