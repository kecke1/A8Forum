using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class ForumChallengeRunMapper
{
    public static ForumChallengeRunDTO ToDto(this ForumChallengeRunViewModel r)
    {
        return new ForumChallengeRunDTO
        {
            Id = r.ForumChallengeRunId,
            Deleted = r.Deleted,
            ForumChallenge = r.ForumChallenge.ToDto(),
            Idate = r.Idate,
            Member = r.Member.ToDto(),
            Post = r.Post,
            Time = r.Time,
            Vehicle = r.Vehicle.ToDto()
        };
    }

    public static ForumChallengeRunViewModel ToForumChallengeRunViewModel(this ForumChallengeRunDTO model)
    {
        return new ForumChallengeRunViewModel
        {
            ForumChallengeRunId = model.Id,
            Deleted = model.Deleted,
            ForumChallenge = model.ForumChallenge.ToForumChallengeViewModel(),
            Idate = model.Idate,
            Member = model.Member.ToMemberViewModel(),
            Post = model.Post,
            Time = model.Time,
            TimeString = model.Time.ToTimeString(),
            Vehicle = model.Vehicle.ToVehicleViewModel()
        };
    }
}