using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class TrackMapper
{
    public static TrackDTO ToDto(this TrackViewModel track)
    {
        return new TrackDTO
        {
            Id = track.TrackId,
            TrackName = track.TrackName
        };
    }

    public static TrackViewModel ToTrackViewModel(this TrackDTO model)
    {
        return new TrackViewModel
        {
            TrackId = model.Id,
            TrackName = model.TrackName
        };
    }
}