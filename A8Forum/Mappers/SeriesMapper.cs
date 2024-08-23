using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class SeriesMapper
{
    public static SeriesDTO ToDto(this ViewModels.SeriesViewModel r)
    {
        return new SeriesDTO
        {
            Id = r.SeriesId,
            EndDate = r.EndDate,
            StartDate = r.StartDate,
        };
    }

    public static ViewModels.SeriesViewModel ToSeriesViewModel(this SeriesDTO model,
        string leaderboard, string leaderboardHtml)
    {
        return new ViewModels.SeriesViewModel
        {
            SeriesId = model.Id,
            EndDate = model.EndDate,
            StartDate = model.StartDate,
            Leaderboard = leaderboard,
            LeaderboardHtml = leaderboard.ToHtml()
        };
    }
}