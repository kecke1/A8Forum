using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class SeasonDTOMapper
{
    public static SeasonDTO ToDto(this Season season)
    {
        return new SeasonDTO
        {
            Id = season.Id,
            SeasonName = season.SeasonName
        };
    }

    public static Season ToSeasonEntity(this SeasonDTO model)
    {
        var s = new Season
        {
            SeasonName = model.SeasonName
        };

        if (!string.IsNullOrEmpty(model.Id))
            s.Id = model.Id;

        return s;
    }
}