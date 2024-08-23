using Shared.Dto;

namespace A8Forum.Mappers;

internal static class TrackMapper
{
    public static TrackDTO ToDto(this ViewModels.TrackViewModel track)
    {
        return new TrackDTO
        {
            Id = track.TrackId,
            TrackName = track.TrackName
        };
    }

    public static ViewModels.TrackViewModel ToTrackViewModel(this TrackDTO model)
    {
        return new ViewModels.TrackViewModel
        {
            TrackId = model.Id,
            TrackName = model.TrackName
        };
    }
}