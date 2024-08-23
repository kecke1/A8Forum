using Shared.Dto;

namespace Shared.Mappers
{
    internal static class SeriesDTOMapper
    {
        public static SeriesDTO ToDto(this Shared.Models.Series series)
        {
            return new SeriesDTO
            {
                Id = series.Id,
                StartDate = series.StartDate,
                EndDate = series.EndDate
            };
        }

        public static Shared.Models.Series ToSeriesEntity(this SeriesDTO model)
        {
            var s = new Shared.Models.Series
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                s.Id = model.Id;
            }

            return s;
        }
    }
}