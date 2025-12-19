using A8Forum.Extensions;
using A8Forum.ViewModels;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Mappers;

internal static class EditSprintRunMapper
{
    public static EditSprintRunDTO ToDto(this EditSprintRunViewModel r)
    {
        return new EditSprintRunDTO
        {
            Id = r.SprintRunId,
            Deleted = r.Deleted,
            Idate = r.Idate,
            MediaLink = r.MediaLink?.CheckAndFormatUrl(),
            MemberId = r.MemberId,
            PostUrl = r.PostUrl.CheckAndFormatUrl(),
            RunDate = r.RunDate,
            Time = r.TimeString.FromTimestringToInt(),
            VehicleId = r.VehicleId,
            TrackId = r.TrackId,
            VipLevel = r.VipLevel,
            Glitch = r.Glitch,
            Shortcut = r.Shortcut
        };
    }

    public static EditSprintRunViewModel ToEditSprintRunViewModel(this SprintRunDTO model)
    {
        return new EditSprintRunViewModel
        {
            SprintRunId = model.Id,
            TrackId = model.Track?.Id ?? "",
            Deleted = model.Deleted,
            Idate = model.Idate,
            MediaLink = model.MediaLink,
            MemberId = model.Member?.Id,
            PostUrl = model.PostUrl,
            RunDate = model.RunDate,
            Time = model.Time,
            TimeString = model.Time.ToTimeString(),
            VehicleId = model.Vehicle?.Id ?? "",
            VipLevel = model.VipLevel,
            Shortcut = model.Shortcut,
            Glitch = model.Glitch
        };
    }
}