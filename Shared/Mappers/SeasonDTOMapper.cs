using Shared.Dto;

namespace Shared.Mappers
{
    internal static class SeasonDTOMapper
    {
        public static SeasonDTO ToDto(this Shared.Models.Season season)
        {
            return new SeasonDTO
            {
                Id = season.Id,
                SeasonName = season.SeasonName
            };
        }

        public static Shared.Models.Season ToSeasonEntity(this SeasonDTO model)
        {
            var s = new Shared.Models.Season
            {
                SeasonName = model.SeasonName
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                s.Id = model.Id;
            }

            return s;
        }
    }
}