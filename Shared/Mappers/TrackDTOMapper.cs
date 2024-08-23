using Shared.Dto;

namespace Shared.Mappers
{
    internal static class TrackDTOMapper
    {
        public static TrackDTO ToDto(this Shared.Models.Track track)
        {
            return new TrackDTO
            {
                Id = track.Id,
                TrackName = track.TrackName
            };
        }

        public static Shared.Models.Track ToTrackEntity(this TrackDTO model)
        {
            var t = new Shared.Models.Track
            {
                TrackName = model.TrackName
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                t.Id = model.Id;
            }

            return t;
        }
    }
}