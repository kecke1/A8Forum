using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class EditForumChallengeRunMapper
{
    public static EditForumChallengeRunDTO ToDto(this EditForumChallengeRunViewModel r)
    {
        return new EditForumChallengeRunDTO
        {
            Id = r.ForumChallengeRunId,
            Deleted = r.Deleted,
            ForumChallengeId = r.ForumChallengeId,
            Idate = r.Idate,
            MemberId = r.MemberId,
            Post = r.Post,
            Time = r.Time,
            VehicleId = r.VehicleId
        };
    }

    public static EditForumChallengeRunViewModel ToEditForumChallengeRunViewModel(this ForumChallengeRunDTO model)
    {
        return new EditForumChallengeRunViewModel
        {
            ForumChallengeRunId = model.Id,
            Deleted = model.Deleted,
            ForumChallengeId = model.ForumChallenge.Id,
            Idate = model.Idate,
            MemberId = model.Member.Id,
            Post = model.Post,
            Time = model.Time,
            TimeString = model.Time.ToTimeString(),
            VehicleId = model.Vehicle.Id
        };
    }
}