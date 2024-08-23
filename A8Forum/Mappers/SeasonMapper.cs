using Shared.Dto;

namespace A8Forum.Mappers;

internal static class SeasonMapper
{
    public static SeasonDTO ToDto(this ViewModels.SeasonViewModel season)
    {
        return new SeasonDTO
        {
            Id = season.SeasonId,
            SeasonName = season.SeasonName
        };
    }

    public static ViewModels.SeasonViewModel ToSeasonViewModel(this SeasonDTO model)
    {
        return new ViewModels.SeasonViewModel
        {
            SeasonName = model.SeasonName,
            SeasonId = model.Id
        };
    }
}