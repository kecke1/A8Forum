using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class SprintTrackReferencePointMapper
{
    public static SprintTrackReferencePointDto ToDto(this SprintTrackReferencePointViewModel r)
    {
        return new SprintTrackReferencePointDto
        {
            Id = r.SprintTrackReferencePointId,
            Date = r.Date,
            Track = r.Track.ToDto()
        };
    }

    public static SprintTrackReferencePointViewModel ToSprintTrackReferencePointViewModel(this SprintTrackReferencePointDto model)
    {
        return new SprintTrackReferencePointViewModel
        {
            SprintTrackReferencePointId = model.Id,
            Date = model.Date,
            Track = model.Track.ToTrackViewModel()
        };
    }
}