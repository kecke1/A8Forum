using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class TrackDTOMapper
{
    public static TrackDTO ToDto(this Track track)
    {
        return new TrackDTO
        {
            Id = track.Id,
            TrackName = track.TrackName
        };
    }

    public static Track ToTrackEntity(this TrackDTO model)
    {
        var t = new Track
        {
            TrackName = model.TrackName
        };

        if (!string.IsNullOrEmpty(model.Id))
            t.Id = model.Id;

        return t;
    }
}