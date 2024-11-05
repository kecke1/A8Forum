using A8Forum.Dto;
using Shared.Dto;
using System.Diagnostics;

namespace A8Forum.Extensions;

public static class GauntletLeaderboardRowDtoExtensions
{
    public static IList<GauntletLeaderboardTableColDto> ToTableCols(
        this IOrderedEnumerable<GauntletLeaderboardRowDto> lb)
    {
        return lb.Select((x, i) => new GauntletLeaderboardTableColDto
            {
                Date = x.RunDate?.ToString("dd.MM.yyyy") ?? "",
                Name = x.MemberDisplayName,
                Pos = x.Position.ToString(),
                Time = GetATag(x.TimeString, x.PostUrl),
                Track = x.TrackName,
                Vehicle1 = GetATag(x.VehicleName1, x.VehicleUrl1),
                Vehicle2 = GetATag(x.VehicleName2, x.VehicleUrl2),
                Vehicle3 = GetATag(x.VehicleName3, x.VehicleUrl3),
                Vehicle4 = GetATag(x.VehicleName4, x.VehicleUrl4),
                Vehicle5 = GetATag(x.VehicleName5, x.VehicleUrl5),
            Verified = $"{(x.A8Plus ? "🍎" : "")}{(x.LapTimeVerified ? "✅" : "")}",
                Video = string.IsNullOrEmpty(x.MediaLink) ? "" : GetATag("🎦", x.MediaLink)
            })
            .ToList();
    }

    private static string GetATag(string text, string link)
    {
        if (string.IsNullOrEmpty(link))
            return text;
        return string.IsNullOrEmpty(text) ? "" : $"<a href='{link}'>{text}</a>";
    }
}