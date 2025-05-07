using A8Forum.Dto;
using Shared.Dto;
using Shared.Extensions;

namespace A8Forum.Extensions;

public static class SprintLeaderboardRowDtoExtensions
{
    public static IList<SprintLeaderboardTableColDto> ToTableCols(
        this IOrderedEnumerable<SprintLeaderboardRowDto> lb, bool showAllRuns = false)
    {

        if (!showAllRuns)
        {
            return lb.Select((x, i) => new SprintLeaderboardTableColDto
                {
                    Date = x.RunDate?.ToString("dd.MM.yyyy") ?? "",
                    Name = x.MemberDisplayName,
                    Pos = x.Position.ToString(),
                    Time = GetATag(x.TimeString, x.PostUrl),
                    Track = x.TrackName,
                    Vehicle = GetATag(x.VehicleName, x.VehicleUrl),
                    Video = string.IsNullOrEmpty(x.MediaLink) ? "" : GetATag("🎦", x.MediaLink),
                    VIP = x.Vip.HasValue && x.Vip > 11 ? x.Vip.ToString() : ""
                })
                .ToList();
        }


        return lb.SelectMany(x => x.Runs)
            .OrderBy(x => x.Track.TrackName)
            .ThenBy(x => x.Time)
            .Select(x => new SprintLeaderboardTableColDto
            {
                Date = x.RunDate?.ToString("dd.MM.yyyy") ?? "",
                Name = x.Member.MemberDisplayName,
                Pos = "",
                Time = GetATag(x.Time.ToTimeString(), x.PostUrl),
                Track = x.Track.TrackName,
                Vehicle = GetATag(x.Vehicle.Name, x.Vehicle.Url),
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