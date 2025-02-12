using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class SprintRunDTOMapper
{
    public static SprintRunDTO ToDto(this SprintRun Sprintrun,
        TrackDTO t,
        VehicleDTO v,
        MemberDTO m)
    {
        return new SprintRunDTO
        {
            Id = Sprintrun.Id,
            Time = Sprintrun.Time,
            Idate = Sprintrun.Idate,
            Deleted = Sprintrun.Deleted,
            Track = t,
            Vehicle = v,
            Member = m,
            PostUrl = Sprintrun.PostUrl,
            RunDate = Sprintrun.RunDate,
            MediaLink = Sprintrun.MediaLink,
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
        };

        if (!string.IsNullOrEmpty(model.Id))
            r.Id = model.Id;

        return r;
    }
}