using A8Forum.Extensions;
using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class GauntletRunMapper
{
    public static GauntletRunDTO ToDto(this GauntletRunViewModel r)
    {
        return new GauntletRunDTO
        {
            Id = r.GauntletRunId,
            Deleted = r.Deleted,
            Idate = r.Idate,
            LapTimeVerified = r.LapTimeVerified,
            MediaLink = r.MediaLink?.CheckAndFormatUrl(),
            Member = r.Member.ToDto(),
            PostUrl = r.PostUrl.CheckAndFormatUrl(),
            RunDate = r.RunDate,
            Time = r.TimeString.FromTimestringToInt(),
            Vehicle1 = r.Vehicle1.ToDto(),
            Vehicle2 = r.Vehicle2.ToDto(),
            Vehicle3 = r.Vehicle3.ToDto(),
            Vehicle4 = r.Vehicle4.VehicleId != null ? r.Vehicle4?.ToDto() : null,
            Vehicle5 = r.Vehicle5.VehicleId != null ? r.Vehicle5?.ToDto() : null,
            Track = r.Track.ToDto()
        };
    }

    public static GauntletRunViewModel ToGauntletRunViewModel(this GauntletRunDTO model)
    {
        return new GauntletRunViewModel
        {
            GauntletRunId = model.Id,
            Track = model.Track.ToTrackViewModel(),
            Deleted = model.Deleted,
            Idate = model.Idate,
            LapTimeVerified = model.LapTimeVerified,
            A8Plus = model.A8Plus,
            MediaLink = model.MediaLink,
            Member = model.Member.ToMemberViewModel(),
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            Time = model.Time,
            TimeString = model.Time.ToTimeString(),
            Vehicle1 = model.Vehicle1.ToVehicleViewModel(),
            Vehicle2 = model.Vehicle2.ToVehicleViewModel(),
            Vehicle3 = model.Vehicle3.ToVehicleViewModel(),
            Vehicle4 = model.Vehicle4?.ToVehicleViewModel(),
            Vehicle5 = model.Vehicle5?.ToVehicleViewModel()
        };
    }
}