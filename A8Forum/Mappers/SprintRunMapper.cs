using A8Forum.Extensions;
using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class SprintRunMapper
{
    public static SprintRunDTO ToDto(this SprintRunViewModel r)
    {
        return new SprintRunDTO
        {
            Id = r.SprintRunId,
            Deleted = r.Deleted,
            Idate = r.Idate,
            MediaLink = r.MediaLink?.CheckAndFormatUrl(),
            Member = r.Member.ToDto(),
            PostUrl = r.PostUrl.CheckAndFormatUrl(),
            RunDate = r.RunDate,
            Time = r.TimeString.FromTimestringToInt(),
            Vehicle = r.Vehicle.ToDto(),
            Track = r.Track.ToDto(),
            VipLevel = r.VipLevel
        };
    }

    public static SprintRunViewModel ToSprintRunViewModel(this SprintRunDTO model)
    {
        return new SprintRunViewModel
        {
            SprintRunId = model.Id,
            Track = model.Track.ToTrackViewModel(),
            Deleted = model.Deleted,
            Idate = model.Idate,
            MediaLink = model.MediaLink,
            Member = model.Member.ToMemberViewModel(),
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            Time = model.Time,
            TimeString = model.Time.ToTimeString(),
            Vehicle = model.Vehicle.ToVehicleViewModel(),
            VipLevel = model.VipLevel
        };
    }
}