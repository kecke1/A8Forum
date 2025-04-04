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
            TrackName = track.TrackName,
            Sprint = track.Sprint,
            Order = track.Order
        };
    }

    public static TrackViewModel ToTrackViewModel(this TrackDTO model)
    {
        return new TrackViewModel
        {
            TrackId = model.Id,
            TrackName = model.TrackName,
            Sprint = model.Sprint,
            Order = model.Order
        };
    }
}