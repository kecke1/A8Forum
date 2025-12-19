using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class SprintRunDTOMapper
{
    public static SprintRunDTO ToDto(this SprintRun sprintrun,
        TrackDTO t,
        VehicleDTO v,
        MemberDTO m)
    {
        return new SprintRunDTO
        {
            Id = sprintrun.Id,
            Time = sprintrun.Time,
            Idate = sprintrun.Idate,
            Deleted = sprintrun.Deleted,
            Track = t,
            Vehicle = v,
            Member = m,
            PostUrl = sprintrun.PostUrl,
            RunDate = sprintrun.RunDate,
            MediaLink = sprintrun.MediaLink,
            VipLevel = sprintrun.VipLevel,
            Glitch = sprintrun.Glitch,
            Shortcut = sprintrun.Shortcut
        };
    }

    public static SprintRun ToSprintRunEntity(this EditSprintRunDTO model)
    {
        var r = new SprintRun
        {
            Time = model.Time,
            Idate = model.Idate,
            Deleted = model.Deleted,
            TrackId = model.TrackId ?? throw new NullReferenceException(),
            VehicleId = model.VehicleId ?? throw new NullReferenceException(),
            MemberId = model.MemberId ?? throw new NullReferenceException(),
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            MediaLink = model.MediaLink,
            VipLevel = model.VipLevel,
            Glitch = model.Glitch,
            Shortcut = model.Shortcut
        };

        if (!string.IsNullOrEmpty(model.Id))
            r.Id = model.Id;

        return r;
    }

    public static SprintRun ToSprintRunEntity(this SprintRunDTO model)
    {
        var r = new SprintRun
        {
            Time = model.Time,
            Idate = model.Idate,
            Deleted = model.Deleted,
            TrackId = model.Track.Id ?? throw new NullReferenceException(),
            VehicleId = model.Vehicle.Id ?? throw new NullReferenceException(),
            MemberId = model.Member.Id ?? throw new NullReferenceException(),
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            MediaLink = model.MediaLink,
            VipLevel = model.VipLevel,
            Glitch = model.Glitch,
            Shortcut = model.Shortcut
        };

        if (!string.IsNullOrEmpty(model.Id))
            r.Id = model.Id;

        return r;
    }
}