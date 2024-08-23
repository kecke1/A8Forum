using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;
using Shared.Dto;
using Shared.Extensions;
using Shared.Mappers;
using Shared.Models;
using Shared.Options;

namespace Shared.Services;

public class ForumChallengeService : IForumChallengeService
{
    private readonly IRepository<ForumChallenge> _forumChallengeRepository;
    private readonly IRepository<ForumChallengeRun> _forumChallengeRunRepository;
    private readonly IRepository<Series> _seriesRepository;
    private readonly IMasterDataService _masterDataService;
    private readonly string _vehiclesBaseUrl;

    public ForumChallengeService(IRepository<ForumChallenge> forumChallengeRepository,
        IMasterDataService masterDataService,
        IRepository<ForumChallengeRun> forumChallengeRunRepository,
        IRepository<Series> seriesRepository,
        IOptions<A8Options> options)
    {
        _forumChallengeRepository = forumChallengeRepository;
        _masterDataService = masterDataService;
        _forumChallengeRunRepository = forumChallengeRunRepository;
        _seriesRepository = seriesRepository;
        _vehiclesBaseUrl = options.Value.VehiclesBaseUrl;
    }

    public async Task AddForumChallengeAsync(ForumChallengeDTO f)
    {
        await _forumChallengeRepository.CreateAsync(f.ToForumChallengeEntity());
    }

    public async Task AddForumChallengeRunAsync(ForumChallengeRunDTO r)
    {
        await _forumChallengeRunRepository.CreateAsync(r.ToForumChallengeRunEntity());
    }

    public async Task AddSeriesAsync(SeriesDTO s)
    {
        await _seriesRepository.CreateAsync(s.ToSeriesEntity());
    }

    public async Task DeleteForumChallengeAsync(string forumChallengeId)
    {
        var c = await GetForumChallengeAsync(forumChallengeId);
        c.Deleted = true;
        await UpdateForumChallengeAsync(c);
    }

    public async Task DeleteForumChallengeRunAsync(string runId)
    {
        var r = await GetForumChallengeRunAsync(runId);
        r.Deleted = true;
        await UpdateForumChallengeRunAsync(r);
    }

    public async Task DeleteSeriesAsync(string seriesId)
    {
        await _seriesRepository.DeleteAsync(seriesId);
    }

    public async Task<ForumChallengeDTO> GetForumChallengeAsync(string forumChallengeId)
    {
        var c = await _forumChallengeRepository.GetAsync(forumChallengeId);
        var season = await _masterDataService.GetSeasonAsync(c.SeasonId);
        var track = await _masterDataService.GetTrackAsync(c.TrackId);

        return c.ToDto(track, season);
    }

    public IOrderedEnumerable<ForumChallengeLeaderboardDto> GetForumChallengeLeaderBoardDto(IEnumerable<ForumChallengeRunDTO> runs)
    {
        var q = runs
            .GroupBy(x => x.Member.Id, y => y)
            .Select(x =>
            {
                var orderedRaces = x.OrderBy(y => y.Time);
                var bestRace = orderedRaces.First();

                var c = new ForumChallengeLeaderboardDto
                {
                    MemberName = bestRace.Member.MemberName,
                    MemberId = x.Key,
                    MemberDisplayName = bestRace.Member.MemberDisplayName,
                    PostUrl = bestRace.Post ?? "",
                    Time = bestRace.Time,
                    TimeString = bestRace.Time.ToTimeString(),
                    ForumChallengeId = runs.First().ForumChallenge.Id,
                    ChallengeEndDate = runs.First().ForumChallenge.EndDate,
                    VehicleName = bestRace.Vehicle.Name,
                    VehicleUrl = bestRace.Vehicle.Url,
                    ForumChallengeTitleHtml = bestRace.ForumChallenge.TitleHtml,
                    ForumChallengeUrl = bestRace.ForumChallenge.Post,
                    Runs = orderedRaces
                };

                return c;
            })
            .ToArray();

        var p = 1;
        var l = q.GroupBy(x => x.Time, y => y)
            .OrderBy(x => x.Key)
            .SelectMany((x, i) =>
            {
                foreach (var r in x)
                {
                    r.Position = p;
                    r.SeriesPoints = p > 20 ? 0 : 20 - (p - 1);
                }

                p += x.Count();
                return x;
            })
            .OrderBy(x => x.Position);

        return l;
    }

    public string GetForumChallengeLeaderboard(IOrderedEnumerable<ForumChallengeLeaderboardDto> lb)
    {
        var table = $@"[b]Leaderboard[/b][div style=""overflow:auto;""][table style='border-collapse: collapse;font-family: arial, sans-serif;']

{string.Join('\n', lb.Select((x, i) => GetChallengeLeaderBoardTableRow(x, x.Position, i)))}

[/table]
[/div]
        ";

        return table;
    }

    private string GetChallengeLeaderBoardTableRow(ForumChallengeLeaderboardDto c, int position, int index)
    {
        return
            @$"[tr {(index % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}]
{GetTdCellWithBorder($"@{c.MemberName}")}
{GetTdCellWithBorder($"{(!string.IsNullOrEmpty(c.PostUrl) ? $"[a href=\"{c.PostUrl}\"]{c.TimeString}[/a]" : c.TimeString)}")}
{GetTdCellWithBorder($"{(!string.IsNullOrEmpty(c.VehicleUrl) ? $"[a href=\"{_vehiclesBaseUrl}{c.VehicleUrl}\"]{c.VehicleName}[/a]" : c.VehicleName)}")}
[/tr]";
    }

    private string GetTdCellWithBorder(string text)
    {
        return text.GetTdCell("padding:1px;padding-left:7px", true);
    }

    public async Task<IEnumerable<ForumChallengeDTO>> GetForumChallengesAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var c = (await _forumChallengeRepository
            .GetAsync(x => (fromDate == null || x.StartDate >= fromDate) && (toDate == null || x.StartDate <= (toDate ?? DateTime.MinValue).AddDays(1))))
            .ToList();

        var s = (await _masterDataService.GetSeasonsAsync()).ToList();
        var t = (await _masterDataService.GetTracksAsync()).ToList();

        return c.Select(x => x.ToDto(t.Single(y => y.Id == x.TrackId),
            s.Single(y => y.Id == x.SeasonId)))
            .ToList();
    }

    private async Task<IEnumerable<ForumChallengeRunDTO>> GetForumChallengeRunsAsync(ForumChallenge c)
    {
        var r = (await _forumChallengeRunRepository
                .GetAsync(x => x.ForumChallengeId == c.Id))
                .ToList();

        var v = (await _masterDataService.GetVehiclesAsync()).ToList();
        var m = (await _masterDataService.GetMembersAsync()).ToList();
        var t = await _masterDataService.GetTrackAsync(c.TrackId);
        var s = await _masterDataService.GetSeasonAsync(c.SeasonId);

        return r.Select(x => x
        .ToDto(c.ToDto(t, s),
            m.Single(y => y.Id == x.MemberId),
            v.Single(y => y.Id == x.VehicleId)))
            .ToList();
    }

    public async Task<IEnumerable<ForumChallengeRunDTO>> GetForumChallengeRunsAsync(string forumChallengeId)
    {
        var c = await _forumChallengeRepository.GetAsync(forumChallengeId);
        return await GetForumChallengeRunsAsync(c);
    }

    public async Task<ForumChallengeRunDTO> GetForumChallengeRunAsync(string runId)
    {
        var r = await _forumChallengeRunRepository.GetAsync(runId);
        var c = await GetForumChallengeAsync(r.ForumChallengeId);
        var m = await _masterDataService.GetMemberAsync(r.MemberId);
        var v = await _masterDataService.GetVehicleAsync(r.VehicleId);

        return r.ToDto(c, m, v);
    }

    public async Task<IEnumerable<SeriesDTO>> GetSeriesAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var s = (await _seriesRepository
            .GetAsync(x => x.StartDate >= (fromDate ?? DateTime.Now.AddYears(-1)) && x.StartDate <= (toDate ?? DateTime.Now)))
            .ToList();

        return s.Select(x => x.ToDto())
            .ToList();
    }

    public async Task<SeriesDTO> GetSeriesAsync(string seriesId)
    {
        return (await _seriesRepository.GetAsync(seriesId)).ToDto();
    }

    private async Task<IEnumerable<ForumChallengeLeaderboardDto>> GetChallengeLeaderboardsByEndDate(DateTime startDate, DateTime endDate)
    {
        var c = (await _forumChallengeRepository
            .GetAsync(x => !x.Deleted && x.EndDate >= startDate && x.EndDate <= endDate && x.EndDate <= DateTime.Now))
            .ToList()
            .Select(async x => GetForumChallengeLeaderBoardDto(await GetForumChallengeRunsAsync(x)));
        var clb = (await Task.WhenAll(c)).ToList();

        return clb.SelectMany(x => x);
    }

    private IEnumerable<SeriesLeaderboardDto> GetSeriesLeaderBoardForChallenges(
        IEnumerable<ForumChallengeLeaderboardDto> challenges,
        int numberOfRuns)
    {
        var leaderboardWithoutPosition = challenges.GroupBy(x => x.MemberId)
            .Select(x =>
            {
                var l = new SeriesLeaderboardDto();
                l.MemberId = x.First().MemberId;
                l.MemberName = x.First().MemberName;
                l.MemberDisplayName = x.First().MemberDisplayName;
                l.NumberOfRuns = x.Count();
                l.Points = x.Count() <= numberOfRuns
                    ? x.Sum(y => y.SeriesPoints)
                    : x.OrderByDescending(y => y.SeriesPoints).Take(numberOfRuns).Sum(y => y.SeriesPoints);

                l.LatestForumChallengeId = x.Max(y => y.ForumChallengeId);
                l.LatestChallengeEndDate = x.Max(y => y.ChallengeEndDate);
                l.LatestChallengeLapTime =
                    x.FirstOrDefault(y => y.ChallengeEndDate == x.Max(z => z.ChallengeEndDate))?.Time ?? 999999;
                l.LatestChallengeTimeString =
                    l.LatestChallengeLapTime == 999999 ? "-" : l.LatestChallengeLapTime.ToTimeString();

                return l;
            })
            .ToArray();

        var pos = 1;
        var orderedLeaderboardWithoutPosition = leaderboardWithoutPosition
            .GroupBy(x => new { x.Points, x.LatestChallengeEndDate, x.LatestChallengeLapTime })
            .OrderByDescending(x => x.Key.Points)
            .ThenByDescending(x => x.Key.LatestChallengeEndDate)
            .ThenBy(x => x.Key.LatestChallengeLapTime);

        var leaderboard = new List<SeriesLeaderboardDto>();

        foreach (var lbGroup in orderedLeaderboardWithoutPosition)
        {
            foreach (var y in lbGroup)
            {
                y.Position = pos;
                leaderboard.Add(y);
            }

            pos += lbGroup.Count();
        }

        return leaderboard;
    }

    public async Task<string> GetSeriesLeaderboardAsync(string seriesId)
    {
        var series = await _seriesRepository.GetAsync(seriesId);

        if (series == null)
        {
            return "";
        }
        var challengesLb = (await GetChallengeLeaderboardsByEndDate(series.StartDate, series.EndDate))
            .ToList();

        var headers = challengesLb
            .GroupBy(x => x.ForumChallengeId)
            .Select(x => new { EndDate = x.Max(y => y.ChallengeEndDate), ForumChallengeUrl = x.Max(y => y.ForumChallengeUrl), ForumChallengeTitleHtml = x.Max(y => y.ForumChallengeTitleHtml) });

        var numberOfRuns = Convert.ToInt32(Math.Floor(Convert.ToDecimal(headers.Count()) / 2));
        var serieslLb = GetSeriesLeaderBoardForChallenges(challengesLb, numberOfRuns > 4 ? numberOfRuns : 4).OrderBy(x => x.Position);

        var table = @$"[div style=""overflow:auto;""]
[table style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
 [td colspan='3' style=""border: 1px solid #dddddd; padding: 3px;""]&nbsp;[/td]
 {string.Join("\n", headers
 .OrderByDescending(x => x.EndDate)
 .Select(x => $"[td colspan='2' style=\"border: 1px solid #dddddd; padding: 3px;\"] {(!string.IsNullOrEmpty(x.ForumChallengeUrl) ? $"[a href='{x.ForumChallengeUrl}' style='color:white;']{x.ForumChallengeTitleHtml}[/a]" : x.ForumChallengeTitleHtml)}[/td]"))}
[/tr]
[tr]
 [td style=""border: 1px solid #dddddd; padding: 3px;""][p style='margin:0px;']{SpanString("Position")}[/td]
 [td style=""border: 1px solid #dddddd; padding: 3px;""][p style='margin:0px;']{SpanString("Name")}[/td]
 [td style=""border: 1px solid #dddddd; padding: 3px;""][p style='margin:0px;']{SpanString("Points")}[/td]
 {string.Join("\n", headers.OrderByDescending(x => x.EndDate).Select(x => "[td style=\"border: 1px solid #dddddd; padding: 3px;\"]Pos[/td][td style=\"border: 1px solid #dddddd; padding: 3px;\"]Time[/td]"))}
[/tr]
[/thead]
[tbody]
{string.Join("\n", GetLbTableRows(serieslLb, challengesLb))}

[/tbody][/table][/div]";

        return table;
    }

    private string SpanString(string s)
    {
        return $"[p style='margin:0px;'][span style=\"white-space: nowrap;\"]{s}[/span][/p]";
    }

    private IEnumerable<string> GetLbTableRows(IEnumerable<SeriesLeaderboardDto> lb,
        IEnumerable<ForumChallengeLeaderboardDto> challenges)
    {
        return lb.Select((x, i) => @$"
[tr {(i % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}][td style=""border: 1px solid #dddddd; padding: 3px;font-weight:bold;""]{SpanString(x.Position.ToString())}[/td]
[td style=""border: 1px solid #dddddd; padding: 3px;font-weight:bold;""]{SpanString($"@{x.MemberName}")} [/td]
[td style=""border: 1px solid #dddddd; padding: 3px;font-weight:bold;""]{SpanString(x.Points.ToString())}[/td]{GetHtmlResultsForMember(x.MemberId, challenges)}[/tr]");
    }

    private string GetHtmlResultsForMember(string memberId, IEnumerable<ForumChallengeLeaderboardDto> challenges)
    {
        var html = new List<string>();
        foreach (var c in challenges.OrderByDescending(x => x.ChallengeEndDate).Select(x => x.ForumChallengeId).Distinct())
        {
            var runResult = challenges.FirstOrDefault(x => x.ForumChallengeId == c && x.MemberId == memberId);

            html.Add(
                $"[td style=\"border: 1px solid #dddddd; padding: 3px;\"]{runResult?.Position.ToString() ?? "-"}[/td][td style=\"border: 1px solid #dddddd; padding: 3px;\"]{runResult?.TimeString ?? "-"}[/td]");
        }

        return string.Join("\n", html);
    }

    public async Task UpdateForumChallengeAsync(ForumChallengeDTO f)
    {
        await _forumChallengeRepository.UpdateAsync(f.ToForumChallengeEntity());
    }

    public async Task UpdateForumChallengeRunAsync(ForumChallengeRunDTO r)
    {
        await _forumChallengeRunRepository.UpdateAsync(r.ToForumChallengeRunEntity());
    }

    public async Task UpdateSeriesAsync(SeriesDTO s)
    {
        await _seriesRepository.UpdateAsync(s.ToSeriesEntity());
    }
}