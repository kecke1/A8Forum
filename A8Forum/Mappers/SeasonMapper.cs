using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class SeasonMapper
{
    public static SeasonDTO ToDto(this SeasonViewModel season)
    {
        return new SeasonDTO
        {
            Id = season.SeasonId,
            SeasonName = season.SeasonName
        };
    }

    public static SeasonViewModel ToSeasonViewModel(this SeasonDTO model)
    {
        return new SeasonViewModel
        {
            SeasonName = model.SeasonName,
            SeasonId = model.Id
        };
    }
}