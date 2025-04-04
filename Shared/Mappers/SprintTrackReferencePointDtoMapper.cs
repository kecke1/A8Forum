using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class SprintTrackReferencePointDtoMapper
{
    public static SprintTrackReferencePointDto ToDto(this SprintTrackReferencePoint p,
        TrackDTO t)
    {
        return new SprintTrackReferencePointDto
        {
            Id = p.Id,
            Track = t,
            Date = p.Date,
        };
    }

    public static SprintTrackReferencePoint ToSprintTrackReferencePointEntity(this SprintTrackReferencePointDto model)
    {
        var r = new SprintTrackReferencePoint
        {
            TrackId = model.Track.Id,
            Date = model.Date
        };

        if (!string.IsNullOrEmpty(model.Id))
            r.Id = model.Id;

        return r;
    }
}