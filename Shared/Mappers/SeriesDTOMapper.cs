using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class SeriesDTOMapper
{
    public static SeriesDTO ToDto(this Series series)
    {
        return new SeriesDTO
        {
            Id = series.Id,
            StartDate = series.StartDate,
            EndDate = series.EndDate
        };
    }

    public static Series ToSeriesEntity(this SeriesDTO model)
    {
        var s = new Series
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        if (!string.IsNullOrEmpty(model.Id))
            s.Id = model.Id;

        return s;
    }
}