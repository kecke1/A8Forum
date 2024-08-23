using Shared.Dto;

namespace Shared.Mappers
{
    internal static class CareerRaceDTOMapper
    {
        public static CareerRaceDTO ToDto(this Shared.Models.CareerRace careerrace,
            TrackDTO t,
            SeasonDTO s)
        {
            return new CareerRaceDTO
            {
                Id = careerrace.Id,
                CareerRaceType = careerrace.CareerRaceType,
                Track = t,
                Class = careerrace.Class,
                Season = s,
                Limitations = careerrace.Limitations,
                LimitationsDescription = careerrace.LimitationsDescription,
                Row = careerrace.Row,
                Column = careerrace.Column,
            };
        }

        public static Shared.Models.CareerRace ToCareerRaceEntity(this CareerRaceDTO model)
        {
            var r = new Shared.Models.CareerRace
            {
                CareerRaceType = model.CareerRaceType,
                TrackId = model.Track.Id ?? throw new NullReferenceException(),
                Class = model.Class,
                SeasonId = model.Season.Id ?? throw new NullReferenceException(),
                Limitations = model.Limitations,
                LimitationsDescription = model.LimitationsDescription,
                Row = model.Row,
                Column = model.Column,
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                r.Id = model.Id;
            }

            return r;
        }
    }
}