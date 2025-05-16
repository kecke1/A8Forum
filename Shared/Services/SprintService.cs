using System.Diagnostics;
using System.Text;
using F23.StringSimilarity;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Dto;
using Shared.Extensions;
using Shared.Mappers;
using Shared.Models;
using Shared.Options;

namespace Shared.Services;

public class SprintService(IRepository<SprintRun> sprintRunRepository,
        IMasterDataService masterDataService,
        IOptions<A8Options> options,
        IRepository<SprintTrackReferencePoint> sprintTrackReferencePointRepository)
    : ISprintService
{
    private readonly string _vehiclesBaseUrl = options.Value.VehiclesBaseUrl;

    public async Task AddSprintRunAsync(EditSprintRunDTO r)
    {
        await sprintRunRepository.CreateAsync(r.ToSprintRunEntity());
    }

    public async Task DeleteSprintRunAsync(string sprintRunId)
    {
        var r = await GetSprintRunAsync(sprintRunId);
        r.Deleted = true;
        await sprintRunRepository.UpdateAsync(r.ToSprintRunEntity());
    }

    public async Task<IOrderedEnumerable<SprintLeaderboardRowDto>> GetSprintLeaderboardRowsAsync(bool excludeVipRuns = false)
    {
        var races = (await GetSprintRunsAsync())
            .Where(x => !x.Deleted);

        var runs = excludeVipRuns ? races.Where(x => !x.VipLevel.HasValue || x.VipLevel < 12) : races;


          var q = runs.GroupBy(x => new { MemberId = x.Member.Id, TrackId = x.Track.Id }, y => y)
            .Select(x =>
            {
                var orderedRaces = x.OrderBy(y => y.Time);
                var bestRace = orderedRaces.First();

                var c = new SprintLeaderboardRowDto
                {
                    MemberName = bestRace.Member.MemberName,
                    MemberId = x.Key.MemberId,
                    MemberDisplayName = bestRace.Member.MemberDisplayName,
                    PostUrl = bestRace.PostUrl ?? "",
                    MediaLink = bestRace.MediaLink ?? "",
                    Vip = bestRace.VipLevel,
                    RunDate = bestRace.RunDate,
                    Time = bestRace.Time,
                    TimeString = bestRace.Time.ToTimeString(),
                    TrackId = x.Key.TrackId,
                    TrackName = bestRace.Track.TrackName,
                    VehicleName = bestRace.Vehicle.ShortName,
                    VehicleUrl = string.IsNullOrEmpty(bestRace.Vehicle.Url)
                        ? ""
                        : _vehiclesBaseUrl + bestRace.Vehicle.Url,
                    Runs = orderedRaces
                };

                return c;
            })
            .ToArray()
            .OrderBy(x => x.TrackName)
            .ThenBy(x => x.Time);

        var withPositions = new List<SprintLeaderboardRowDto>();
        foreach (var t in q.GroupBy(x => x.TrackId))
        {
            var p = 1;
            var l = t.GroupBy(x => x.Time, y => y)
                .OrderBy(x => x.Key)
                .SelectMany((x, i) =>
                {
                    foreach (var r in x)
                    {
                        r.Position = p;
                        r.PositionPoints = p > 20 ? 0 : 20 - (p - 1);
                    }

                    p += x.Count();
                    return x;
                })
                .OrderBy(x => x.Position);

            withPositions.AddRange(l);
        }

        return withPositions.OrderBy(x => x.TrackName)
            .ThenBy(x => x.Position)
            .ThenBy(x => x.PostDate);
    }

    public async Task<string> GetSprintLeaderboardByMemberAsync(IOrderedEnumerable<SprintLeaderboardRowDto> races)
    {
        var tables = new List<string>();
        foreach (var track in races.GroupBy(x => x.MemberDisplayName).OrderBy(x => x.Key))
            tables.Add(GetSprintLeaderboardTableByMember(track.Select(x => x).OrderBy(x => x.TrackName)));
        var output = @$"[b]Best lap times[/b]

        {string.Join("\n", tables)}";
        return output;
    }

    public async Task<string> GetSprintLeaderboardByTrackAsync(IOrderedEnumerable<SprintLeaderboardRowDto> runs)
    {
        var tables = new List<string>();
        foreach (var track in runs.GroupBy(x => x.TrackName).OrderBy(x => x.Key))
            tables.Add(GetSprintLeaderboardTableByTrack(track.Select(x => x)
                .OrderBy(x => x.Position)
                .ThenByDescending(x => x.RunDate ?? DateTime.MinValue)));

        var output = @$"[b]Leaderboards by track[/b]

{string.Join("\n", tables)}";

        return output;
    }

    public async Task<SprintRunDTO> GetSprintRunAsync(string sprintRunId)
    {
        var r = await sprintRunRepository.GetAsync(sprintRunId);
        var member = await masterDataService.GetMemberAsync(r.MemberId);
        var track = await masterDataService.GetTrackAsync(r.TrackId);
        var vehicles = (await masterDataService.GetVehiclesAsync()).ToArray();

        return r.ToDto(track,
            vehicles.Single(y => y.Id == r.VehicleId),
            member);
    }

    public async Task<IEnumerable<SprintRunDTO>> GetSprintRunsAsync()
    {
        var vehicles = (await masterDataService.GetVehiclesAsync()).ToArray();
        var tracks = (await masterDataService.GetTracksAsync(false, true)).ToArray();
        var members = (await masterDataService.GetMembersAsync()).ToArray();
        var r = (await sprintRunRepository.GetAsync(x => true)).ToArray();

        return r.Select(x => x
                .ToDto(tracks
                        .Single(y => y.Id == x.TrackId),
                    vehicles.Single(y => y.Id == x.VehicleId),
                    members.Single(y => y.Id == x.MemberId)))
            .ToArray();
    }

    public async Task<SprintTrackReferencePointDto?> GetSprintTrackReferencePointAsync()
    {
        var r = await sprintTrackReferencePointRepository
            .GetAsync(x => true)
            .FirstOrDefaultAsync();

        if (r == null)
        {
            return null;
        }

        var t = await masterDataService.GetTrackAsync(r.TrackId);
        return r.ToDto(t);
    }

    public async Task UpsertSprintTrackReferencePointAsync(SprintTrackReferencePointDto r)
    {
        var exist = await GetSprintTrackReferencePointAsync();
        if (exist == null)
        {
            await sprintTrackReferencePointRepository.CreateAsync(r.ToSprintTrackReferencePointEntity());
        }
        else
        {
            await sprintTrackReferencePointRepository.UpdateAsync(r.ToSprintTrackReferencePointEntity());
        }
    }

    public async Task<string> GetSprintTotalLeaderboardTableAsync(IEnumerable<SprintLeaderboardRowDto> races)
    {
        var p = 1;
        var result = races.GroupBy(x => x.MemberId)
            .Select(x => new SprintLeaderboardResultDto
            {
                Name = x.First().MemberName,
                Points = x.Sum(y => y.PositionPoints),
                Position = 0,
                NumberOfTracks = x.Count(),
                AvgPoints = x.Any() ? x.Sum(y => y.PositionPoints) / Convert.ToDouble(x.Count()) : 0,
                NumberOfFirstPositions = x.Count(y => y.Position == 1)
            })
            .GroupBy(x => x.Points, y => y)
            .OrderByDescending(x => x.Key)
            .SelectMany((x, i) =>
            {
                foreach (var r in x)
                    r.Position = p;

                p += x.Count();
                return x;
            })
            .OrderBy(x => x.Position);

        var table =
            $@"[div style=""overflow:auto;""][table style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"Pos.".GetTdHeaderCell()}
{"Points".GetTdHeaderCell()}
{"Name".GetTdHeaderCell()}
{"No of tracks".GetTdHeaderCell()}
{"Avg points/track".GetTdHeaderCell()}
{"No of 1st pos.".GetTdHeaderCell()}
[/tr]
[/thead]
{string.Join('\n', result.Select((x, i) => GetSprintTotalLeaderboardTableRow(x, x.Position, i)))}

[/table][/div]";

        return table;
    }

    public async Task<string> GetSprintTotalLeaderboardPageAsync(IEnumerable<SprintLeaderboardRowDto> races, IEnumerable<SprintLeaderboardRowDto> racesNoVip)
    {

        var table = $@"This is the forum challenge for Sprint TLE, it will go on as long as there is Sprint TLE in A8.

The purpose is, in addition to a friendly competition between forum members, to keep track of everyones best times.

It will hopefully also be a starting point for discussing how to improve lap times in Sprint.

I will maintain leaderboards for every track, and also a total leaderboard where participants will receive points based on their positions on the track leaderboards.

If you want to know the next date for a specific sprint, [a href='https://a8forum.azurewebsites.net/SprintLeaderboard/Schedule']Here is a schedule of upcoming sprints[/a]

[b]Posting lap times[/b]
Everyone is welcome to post their lap times when they set a new personal record for at track.

You are encouraged to provide a video or screenshot in the post, but it is not necessary.

Please include track name, lap time and vehicle name in the post. It makes it a lot easier for me to maintain the leaderboards.
If the run was not made recently, you can also include the date of when it was made.

I would also like you to specify VIP level if you are VIP 12 or above.

[b]Total leaderboard[/b]
The total leaderboard points are the sum of the points given in each track leaderboard. The formula for calculating points for each track is 21 - position. For example, position 1 will give 21 points - 1 = 20 points. Everyone on a track leaderboard will always receive at least 1 point.

[b]Total leaderboard[/b]
[spoiler]
{await GetSprintTotalLeaderboardTableAsync(races)}
[/spoiler]

[b]Total leaderboard without VIP boosted runs[/b]
[spoiler]
{await GetSprintTotalLeaderboardTableAsync(racesNoVip)}
[/spoiler]
";

        return table;
    }

    public async Task ImportSprintRunsAsync(SprintImportDTO races)
    {
        var tracks = (await masterDataService.GetTracksAsync(false, true)).ToArray();
        var vehicles = (await masterDataService.GetVehiclesAsync()).Where(x => x.MaxRank > 1850).ToArray();

        var toCreate = new List<SprintRunDTO>();

        foreach (var row in races.ImportedText.Split('\n'))
        {
            if (string.IsNullOrEmpty(row))
                continue;

            var cols = row.Trim().Split(string.IsNullOrEmpty(races.Seperator) ? "\t" : races.Seperator)
                .Select(x => x.Trim()).ToArray();

            var gd = new SprintRunDTO
            {
                Idate = DateTime.Now,
                Time = cols[races.TimeColumn - 1].FromTimestringToInt(),
                Track = tracks
                    .Where(x => x.TrackName.First() == cols[races.TrackColumn - 1].First())
                    .Where(x => x.TrackName.Last() == cols[races.TrackColumn - 1].Last())
                    .First(x => x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName), cols[races.TrackColumn - 1])),
                Vehicle = ClosestMatch(vehicles, cols[races.VehicleColumn - 1]),
                RunDate = races.RunDateColumn.HasValue && !string.IsNullOrEmpty(races.RunDateFormat)
                    ? DateTime.ParseExact(cols[races.RunDateColumn.Value - 1].Trim(), races.RunDateFormat, null)
                    : null,
                Member = await masterDataService.GetMemberAsync(races.MemberId),
                PostUrl = races.PostUrl
            };


            if (races.MediaLinkColumn.HasValue)
            {
                gd.MediaLink = cols[races.MediaLinkColumn.Value - 1];
            }

            toCreate.Add(gd);
        }

        foreach (var r in toCreate)
            await sprintRunRepository.CreateAsync(r.ToSprintRunEntity());
    }

    public async Task UpdateSprintRunAsync(EditSprintRunDTO r)
    {
        await sprintRunRepository.UpdateAsync(r.ToSprintRunEntity());
    }

    public async Task<SprintReportDTO> GetSprintReportAsync()
    {
        var races = await GetSprintLeaderboardRowsAsync();
        var racesNoVip = await GetSprintLeaderboardRowsAsync(true);
        var byTrack = await GetSprintLeaderboardByTrackAsync(races);
        var byMember = await GetSprintLeaderboardByMemberAsync(races);

        var report = new SprintReportDTO
        {
            LeaderBoardByTrack = byTrack,
            LeaderBoardByMember = byMember,
            TotalLeaderBoard = await GetSprintTotalLeaderboardPageAsync(races, racesNoVip),
            TotalLeaderBoardNoVip = await GetSprintTotalLeaderboardTableAsync(racesNoVip)
        };
        report.LeaderBoardByMemberHtml = report.LeaderBoardByMember.ToHtml();
        report.LeaderBoardByTrackHtml = report.LeaderBoardByTrack.ToHtml();
        report.TotalLeaderBoardHtml = report.TotalLeaderBoard.ToHtml();
        report.TotalLeaderBoardNoVipHtml = report.TotalLeaderBoardNoVip.ToHtml();
        report.LeaderBoardJson = JsonConvert.SerializeObject(races);
        return report;
    }

    public async Task<SprintRunDTO> GetSprintRunFromTemplateAsync(string template, string postUrl)
    {
        var tracks = (await masterDataService.GetTracksAsync(false, true)).ToArray();
        var vehicles = (await masterDataService.GetVehiclesAsync()).Where(x => x.MaxRank > 1850).ToArray();

        if (template.Contains("track:"))
        {

            return new SprintRunDTO
            {
                Time = GetValueFromTemplate("time", template)?.FromTimestringToInt() ?? 0,
                Idate = DateTime.Now,
                Track = tracks.FirstOrDefault(x =>
                    x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName),
                        GetValueFromTemplate("track", template))),
                Vehicle = ClosestMatch(vehicles, GetValueFromTemplate("vehicle", template)),
                Member = null,
                PostUrl = postUrl,
                RunDate = null,
                MediaLink = GetValueFromTemplate("video", template),
            };
        }

        var templateRows = template
            .Split(new char[] { '\n', ';' })
            .Where(x => !string.IsNullOrEmpty(x.Trim()))
            .Select(x => x.Trim())
            .ToList();

        var r = new SprintRunDTO
        {
            Idate = DateTime.Now,
            Member = null,
            RunDate = DateTime.Now,
        };

        if (!templateRows.Any())
        {
            return r;
        }

        var timeSet = false;
        var trackSet = false;
        var vehicleSet = false;
        var mediaLinkSet = false;

        foreach (var row in templateRows)
        {
            if (!trackSet && !char.IsDigit(row.First()))
            {
                r.Track = tracks.FirstOrDefault(x =>
                    x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName), row));
                trackSet = true;
                continue;
            }

            if (!timeSet)
            {
                try
                {
                    r.Time = row.Trim().FromTimestringToInt();
                    timeSet = true;
                    continue;
                }
                catch
                {
                }
            }

            if (!mediaLinkSet && row.Contains("/") && row.Contains("."))
            {
                r.MediaLink = row;
                mediaLinkSet = true;
                continue;
            }

            if (!vehicleSet)
            {
                r.Vehicle = ClosestMatch(vehicles, row);
                vehicleSet = true;
                continue;
            }
        }

        var referencePoint = await GetSprintTrackReferencePointAsync();
        r.RunDate = GetLatestTrackDate(DateTime.Now, r.Track, tracks, referencePoint);

        return r;
    }

    private DateTime GetLatestTrackDate(DateTime startDate, TrackDTO track, IEnumerable<TrackDTO> tracks, SprintTrackReferencePointDto reference)
    {
        var found = false;
        do
        {
            if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(-2);
            }

            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(-1);
            }

            if (GetSprintTrackByDate(startDate, tracks, reference).Id == track.Id)
            {
                found = true;
                return startDate;
            }

            startDate = startDate.AddDays(-1);


        } while (found == false);

        return startDate;
    }


    private string? GetValueFromTemplate(string key, string template)
    {
        foreach (var row in template.Split('\n'))
        {
            var cols = row.Split(':', 2);
            if (cols.Length == 2 && cols.First().ToLower().Contains(key))
                return cols.Last();
        }

        return null;
    }

    private string GetSprintTotalLeaderboardTableRow(SprintLeaderboardResultDto g, int position, int index)
    {
        return
            @$"[tr {(index % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}]
{$"{position}.".GetTdCell("padding:1px;padding-left:7px")}
{$"{g.Points}".GetTdCell("padding:1px;padding-left:7px")}
{$"@{g.Name}".GetTdCell("padding:1px;padding-left:7px")}
{$"{g.NumberOfTracks}".GetTdCell("padding:1px;padding-left:7px")}
{$"{Math.Round(g.AvgPoints, 1)}".GetTdCell("padding:1px;padding-left:7px")}
{$"{g.NumberOfFirstPositions}".GetTdCell("padding:1px;padding-left:7px")}
[/tr]";
    }

    private string GetSprintLeaderboardTableByMember(IOrderedEnumerable<SprintLeaderboardRowDto> runs)
    {
        var output =
            @$"[b]@{runs.First().MemberName}[/b][spoiler][div style=""overflow:auto;""][table class=""list"" style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"Track".GetTdHeaderCell()}
{"Time".GetTdHeaderCell()}
{"Date".GetTdHeaderCell()}
{"Video".GetTdHeaderCell("", false)}
{"Vehicle".GetTdHeaderCell()}
{"VIP".GetTdHeaderCell()}
[/tr]
[/thead]
{string.Join('\n', runs.Select((x, i) => GetSprintLeaderboardTableRowByMember(x, i)))}

[/table][/div]
[/spoiler]";

        return output;
    }


    private string GetSprintLeaderboardTableRowByMember(SprintLeaderboardRowDto race, int i)
    {
        return $@"[tr]
{GetTdCellWithBorder(race.TrackName)}
{GetTdCellWithBorder(race.TimeString)}
{GetTdCellWithBorder(race.RunDate.HasValue ? race.RunDate.Value.ToString("dd.MM.yyyy") : "")}
{GetTdCellWithBorder(!string.IsNullOrEmpty(race.MediaLink) ? GetLinkOrText(race.MediaLink, "🎦") : "", false)}
{GetSprintTableVehicleCell(race.VehicleUrl, race.VehicleName, true)}
{GetTdCellWithBorder(race.Vip.HasValue && race.Vip > 11 ? race.Vip.ToString() : "")}
[/tr]";
    }

    private string GetTdCellWithBorder(string text = "", bool spanText = true, bool emptyCellIfMissing = true)
    {
        return text.GetTdCell("", spanText, emptyCellIfMissing);
    }

    private string GetLinkOrText(string linkUrl, string text)
    {
        if (string.IsNullOrEmpty(linkUrl))
            return text;
        return $"[a href=\"{linkUrl}\"]{text}[/a]";
    }

    private string GetSprintTableVehicleCell(string url, string name, bool emptyCellIfMissing)
    {
        return GetTdCellWithBorder(name, true, emptyCellIfMissing);
    }

    private string GetSprintLeaderboardTableRowByTrack(SprintLeaderboardRowDto race, int i)
    {
        return $@"[tr]

{GetTdCellWithBorder($"{race.Position}.", false)}
{GetTdCellWithBorder($"@{race.MemberName}")}
{GetTdCellWithBorder(race.TimeString)}
{GetTdCellWithBorder(race.RunDate.HasValue ? race.RunDate.Value.ToString("dd.MM.yyyy") : "", race.RunDate.HasValue)}
{GetTdCellWithBorder(!string.IsNullOrEmpty(race.MediaLink) ? GetLinkOrText(race.MediaLink, "🎦") : "", false)}
{GetSprintTableVehicleCell(race.VehicleUrl, race.VehicleName, true)}
{GetTdCellWithBorder(race.Vip.HasValue && race.Vip > 11 ? race.Vip.ToString() : "")}
[/tr]";
    }

    private string GetSprintLeaderboardTableByTrack(IOrderedEnumerable<SprintLeaderboardRowDto> races)
    {
        return
            @$"[b]{races.First().TrackName}[/b][spoiler][div style=""overflow:auto;""][table class=""list"" style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"Pos.".GetTdHeaderCell()}
{"Name".GetTdHeaderCell()}
{"Time".GetTdHeaderCell()}
{"Date".GetTdHeaderCell()}
{"Video".GetTdHeaderCell()}
{"Vehicle".GetTdHeaderCell()}
{"VIP".GetTdHeaderCell()}
[/tr]
[/thead]
{string.Join('\n', races.Select((x, i) => GetSprintLeaderboardTableRowByTrack(x, i)))}

[/table][/div]
[/spoiler]";
    }


    private TrackDTO GetSprintTrackByDate(DateTime date, IEnumerable<TrackDTO> tracks, SprintTrackReferencePointDto reference)
    {
        var o = tracks.OrderBy(x => x.Order).ToList();
        // Get number of weekdays between date and reference point
        var diff = (date.Date > reference.Date ? reference.Date.BusinessDaysUntil(date.Date) : date.Date.BusinessDaysUntil(reference.Date))  -1;
        
        var s = date.Date < reference.Date ? reference.Track.Order - diff % 40 : (reference.Track.Order + diff) % 40;

        var t = s switch
        {
            0 => tracks.Single(x => x.Order == 40),
            < 0 => tracks.Single(x => x.Order == 40 + s + reference.Track.Order),
            _ => tracks.Single(x => x.Order == s)
        };

        return t;
    }

    private (TrackDTO track, List<DateTime?> dates) GetTrackSchedule(TrackDTO t, DateTime startDate)
    {
        return (t,
            new List<DateTime?>
            {
                startDate, startDate.AddDays(56), startDate.AddDays(56 * 2), startDate.AddDays(56 * 3),
                startDate.AddDays(56 * 4), startDate.AddDays(56 * 5)
            });
    }

    public async Task<SprintScheduleDTO> GetSprintScheduleAsync(DateTime startDate)
    {
        if (startDate.DayOfWeek == DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(2);
        }

        if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
            startDate = startDate.AddDays(1);
        }

        var tracks = (await masterDataService.GetTracksAsync(false, true)).ToList();
        var reference = await GetSprintTrackReferencePointAsync();

        var today = GetSprintTrackByDate(startDate, tracks, reference);

        var s = new SprintScheduleDTO();

        var day = 0;
        foreach (var t in tracks.Where(x => x.Order >= today.Order).OrderBy(x => x.Order))
        {
            while (true)
            {
                if (t.Id == GetSprintTrackByDate(startDate.AddDays(day), tracks, reference).Id)
                {
                    break;
                }

                day++;
            }

            s.Schedule.Add(GetTrackSchedule(t, startDate.AddDays(day)));
            day++;
        }

        foreach (var t in tracks.Where(x => x.Order < today.Order).OrderBy(x => x.Order))
        {
            while (true)
            {
                if (t.Id == GetSprintTrackByDate(startDate.AddDays(day), tracks, reference).Id)
                {
                    break;
                }
                day++;
            }

            s.Schedule.Add(GetTrackSchedule(t, startDate.AddDays(day)));
            day++;
        }

        return s;
    }


    private string ClosestMatch(IEnumerable<string> m, string s)
    {
        //var distance = int.MaxValue;//
        var distance = double.MaxValue;
        var result = "";
        var lcs = new LongestCommonSubsequence();

        foreach (var t in m
                     .Where(x => x.ToLower().StartsWith(s.ToLower().First()) 
                                 && x.ToLower().EndsWith(s.ToLower().Last())))
        {
            var d = lcs.Distance(t.ToLower(), s.ToLower());
            // var d = Fastenshtein.Levenshtein.Distance(t, s);
            if (d < distance)
            {
                distance = d;
                result = t;
            }
        }

        return result;
    }

    private VehicleDTO? ClosestMatch(IEnumerable<VehicleDTO> vehicles, string? s)
    {
        if (s == null)
            return null;
        //var distance = int.MaxValue;//
        var distance = double.MaxValue;
        var result = vehicles.First();
        var lcs = new LongestCommonSubsequence();

        foreach (var v in vehicles)
        {
            var shortest = lcs.Distance(v.ShortName, s);

            foreach (var d in v.Keyword.Split(';').Where(x => !string.IsNullOrEmpty(x)))
            {
                var kwDistance = lcs.Distance(d.Trim(), s);
                if (kwDistance < shortest)
                    shortest = kwDistance;
            }

            // var d = Fastenshtein.Levenshtein.Distance(t, s);
            if (shortest < distance)
            {
                distance = shortest;
                result = v;
            }
        }

        return result;
    }

    private class SprintLeaderboardResultDto
    {
        public required string Name { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
        public int NumberOfTracks { get; set; }
        public double AvgPoints { get; set; }
        public int NumberOfFirstPositions { get; set; }
    }
}