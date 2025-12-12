using A8Forum.Dto;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Extensions;

public static class GauntletLeaderboardRowDtoExtensions
{
    public static IList<GauntletLeaderboardTableColDto> ToTableCols(
        this IOrderedEnumerable<GauntletLeaderboardRowDto> lb, bool showAllRuns = false)
    {

        if (!showAllRuns)
        {
            return lb.Select((x, i) => new GauntletLeaderboardTableColDto
                {
                    Date = x.RunDate?.ToString("dd.MM.yyyy") ?? "",
                    Name = x.MemberDisplayName,
                    Time = GetATag(x.TimeString, x.PostUrl),
                    Track = x.TrackName,
                    Vehicle1 = GetATag(x.VehicleName1, x.VehicleUrl1),
                    Vehicle2 = GetATag(x.VehicleName2, x.VehicleUrl2),
                    Vehicle3 = GetATag(x.VehicleName3, x.VehicleUrl3),
                    Vehicle4 = GetATag(x.VehicleName4, x.VehicleUrl4),
                    Vehicle5 = GetATag(x.VehicleName5, x.VehicleUrl5),
                    Verified = $"{(x.A8Plus ? "🍎" : "")}{(x.LapTimeVerified ? "✅" : "")}",
                    Video = string.IsNullOrEmpty(x.MediaLink) ? "" : GetATag("🎦", x.MediaLink),
                    VIP = x.VipLevel.HasValue && x.VipLevel > 11 ? x.VipLevel.ToString() : ""

            })
                .ToList();
        }


        return lb.SelectMany(x => x.Runs)
            .OrderBy(x => x.Track.TrackName)
            .ThenBy(x => x.Time)
            .Select(x => new GauntletLeaderboardTableColDto
            {
                Date = x.RunDate?.ToString("dd.MM.yyyy") ?? "",
                Name = x.Member.MemberDisplayName,
                Time = GetATag(x.Time.ToTimeString(), x.PostUrl),
                Track = x.Track.TrackName,
                Vehicle1 = GetATag(x.Vehicle1.Name, x.Vehicle1.Url),
                Vehicle2 = GetATag(x.Vehicle2.Name, x.Vehicle2.Url),
                Vehicle3 = GetATag(x.Vehicle3.Name, x.Vehicle3.Url),
                Vehicle4 = x.Vehicle4 != null ? GetATag(x.Vehicle4.Name, x.Vehicle4.Url) : "",
                Vehicle5 = x.Vehicle5 != null ? GetATag(x.Vehicle5.Name, x.Vehicle5.Url) : "",
                Verified = $"{(x.A8Plus ? "🍎" : "")}{(x.LapTimeVerified ? "✅" : "")}",
                Video = string.IsNullOrEmpty(x.MediaLink) ? "" : GetATag("🎦", x.MediaLink),
                VIP = x.VipLevel.HasValue && x.VipLevel > 11 ? x.VipLevel.ToString() : ""
            })
            .ToList();
            
    }

    private static string GetATag(string text, string link)
    {
        if (string.IsNullOrEmpty(link))
            return text;
        return string.IsNullOrEmpty(text) ? "" : $"<a href='{link}' target='_blank'>{text}</a>";
    }
}