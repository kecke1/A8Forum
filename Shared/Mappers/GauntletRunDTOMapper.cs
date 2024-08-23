using Shared.Dto;

namespace Shared.Mappers
{
    internal static class GauntletRunDTOMapper
    {
        public static GauntletRunDTO ToDto(this Shared.Models.GauntletRun gauntletrun,
            TrackDTO t,
            VehicleDTO v1,
            VehicleDTO v2,
            VehicleDTO v3,
            VehicleDTO? v4,
            VehicleDTO? v5,
            MemberDTO m)
        {
            return new GauntletRunDTO
            {
                Id = gauntletrun.Id,
                Time = gauntletrun.Time,
                Idate = gauntletrun.Idate,
                Deleted = gauntletrun.Deleted,
                Track = t,
                Vehicle1 = v1,
                Vehicle2 = v2,
                Vehicle3 = v3,
                Vehicle4 = v4,
                Vehicle5 = v5,
                Member = m,
                PostUrl = gauntletrun.PostUrl,
                RunDate = gauntletrun.RunDate,
                MediaLink = gauntletrun.MediaLink,
                LapTimeVerified = gauntletrun.LapTimeVerified
            };
        }

        public static Shared.Models.GauntletRun ToGauntletRunEntity(this GauntletRunDTO model)
        {
            var r = new Shared.Models.GauntletRun
            {
                Time = model.Time,
                Idate = model.Idate,
                Deleted = model.Deleted,
                TrackId = model.Track.Id ?? throw new NullReferenceException(),
                Vehicle1Id = model.Vehicle1.Id ?? throw new NullReferenceException(),
                Vehicle2Id = model.Vehicle2.Id ?? throw new NullReferenceException(),
                Vehicle3Id = model.Vehicle3.Id ?? throw new NullReferenceException(),
                Vehicle4Id = model.Vehicle4?.Id,
                Vehicle5Id = model.Vehicle5?.Id,
                MemberId = model.Member.Id ?? throw new NullReferenceException(),
                PostUrl = model.PostUrl,
                RunDate = model.RunDate,
                MediaLink = model.MediaLink,
                LapTimeVerified = model.LapTimeVerified
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                r.Id = model.Id;
            }

            return r;
        }
    }
}