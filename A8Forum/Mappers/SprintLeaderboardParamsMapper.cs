using A8Forum.ViewModels;
using Shared.Params;

namespace A8Forum.Mappers
{
    public static class SprintLeaderboardParamsMapper
    {
        public static GetSprintLeaderboardRowsParams ToParams(LeaderboardFilterInput f)
        {
            return new GetSprintLeaderboardRowsParams
            {
                MinVipLevel = f.VipLevelMin ?? 0,
                MaxVipLevel = f.VipLevelMax ?? 15,
                Date = f.UseLeaderboardDate ? f.LeaderboardDate : null
            };
        }
    }
}

