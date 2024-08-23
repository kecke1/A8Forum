using Shared.Dto;

namespace A8Forum.Mappers;

internal static class ForumChallengeMapper
{
    public static ForumChallengeDTO ToDto(this ViewModels.ForumChallengeViewModel r)
    {
        return new ForumChallengeDTO
        {
            Id = r.ForumChallengeId,
            EndDate = r.EndDate,
            StartDate = r.StartDate,
            CustomTitle = r.CustomTitle,
            Deleted = r.Deleted,
            Idate = r.Idate,
            MaxRank = r.MaxRank,
            Post = r.Post,
            Season = r.Season.ToDto(),
            Track = r.Track.ToDto()
        };
    }

    public static ViewModels.ForumChallengeViewModel ToForumChallengeViewModel(this ForumChallengeDTO model)
    {
        return new ViewModels.ForumChallengeViewModel
        {
            ForumChallengeId = model.Id,
            EndDate = model.EndDate,
            StartDate = model.StartDate,
            CustomTitle = model.CustomTitle,
            Deleted = model.Deleted,
            Idate = model.Idate,
            MaxRank = model.MaxRank,
            Post = model.Post,
            Season = model.Season.ToSeasonViewModel(),
            Track = model.Track.ToTrackViewModel(),
            LeaderboardBB = model.Leaderboard,
            LeaderboardHtml = ""
        };
    }
}