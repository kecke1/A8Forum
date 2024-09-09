using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class ForumChallengeDTOMapper
{
    public static ForumChallengeDTO ToDto(this ForumChallenge forumchallenge,
        TrackDTO t,
        SeasonDTO s)
    {
        return new ForumChallengeDTO
        {
            Id = forumchallenge.Id,
            StartDate = forumchallenge.StartDate,
            EndDate = forumchallenge.EndDate,
            Idate = forumchallenge.Idate,
            Post = forumchallenge.Post,
            CustomTitle = forumchallenge.CustomTitle,
            Deleted = forumchallenge.Deleted,
            MaxRank = forumchallenge.MaxRank,
            Track = t,
            Season = s
        };
    }

    public static ForumChallenge ToForumChallengeEntity(this ForumChallengeDTO model)
    {
        var f = new ForumChallenge
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Idate = model.Idate,
            Post = model.Post,
            CustomTitle = model.CustomTitle,
            Deleted = model.Deleted,
            MaxRank = model.MaxRank,
            TrackId = model.Track.Id ?? throw new NullReferenceException(),
            SeasonId = model.Season.Id ?? throw new NullReferenceException()
        };

        if (!string.IsNullOrEmpty(model.Id))
            f.Id = model.Id;

        return f;
    }
}