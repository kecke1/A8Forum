using A8Forum.Extensions;
using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class EditGauntletRunMapper
{
    public static EditGauntletRunDTO ToDto(this EditGauntletRunViewModel r)
    {
        return new EditGauntletRunDTO
        {
            Id = r.GauntletRunId,
            Deleted = r.Deleted,
            Idate = r.Idate,
            LapTimeVerified = r.LapTimeVerified,
            A8Plus = r.A8Plus,
            MediaLink = r.MediaLink?.CheckAndFormatUrl(),
            MemberId = r.MemberId,
            PostUrl = r.PostUrl.CheckAndFormatUrl(),
            RunDate = r.RunDate,
            Time = r.TimeString.FromTimestringToInt(),
            Vehicle1Id = r.Vehicle1Id,
            Vehicle2Id = r.Vehicle2Id,
            Vehicle3Id = r.Vehicle3Id,
            Vehicle4Id = r.Vehicle4Id,
            Vehicle5Id = r.Vehicle5Id,
            TrackId = r.TrackId,
            VipLevel = r.VipLevel,
            Shortcut = r.Shortcut,
            Glitch = r.Glitch
        };
    }

    public static EditGauntletRunViewModel ToEditGauntletRunViewModel(this GauntletRunDTO model)
    {
        return new EditGauntletRunViewModel
        {
            GauntletRunId = model.Id,
            TrackId = model.Track?.Id ?? "",
            Deleted = model.Deleted,
            Idate = model.Idate,
            LapTimeVerified = model.LapTimeVerified,
            A8Plus = model.A8Plus,
            MediaLink = model.MediaLink,
            MemberId = model.Member?.Id,
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            Time = model.Time,
            VipLevel = model.VipLevel,
            TimeString = model.Time.ToTimeString(),
            Vehicle1Id = model.Vehicle1?.Id ?? "",
            Vehicle2Id = model.Vehicle2?.Id ?? "",
            Vehicle3Id = model.Vehicle3?.Id ?? "",
            Vehicle4Id = model.Vehicle4?.Id,
            Vehicle5Id = model.Vehicle5?.Id,
            Glitch = model.Glitch,
            Shortcut = model.Shortcut
        };
    }
}