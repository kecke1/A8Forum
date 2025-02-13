﻿using System.Text;
using F23.StringSimilarity;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
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
        IOptions<A8Options> options)
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

    public async Task<IOrderedEnumerable<SprintLeaderboardRowDto>> GetSprintLeaderboardRowsAsync()
    {
        var races = (await GetSprintRunsAsync())
            .Where(x => !x.Deleted);

        var q = races
            .GroupBy(x => new { MemberId = x.Member.Id, TrackId = x.Track.Id }, y => y)
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
                AvgPoints = x.Any() ? x.Sum(y => y.PositionPoints) / Convert.ToDouble(x.Count()) : 0
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

        var table = $@"This is the forum challenge for Sprint TLE, it will go on as long as there is Sprint TLE in A8.

The purpose is, in addition to a friendly competition between forum members, to keep track of everyones best times.

It will hopefully also be a starting point for discussing how to improve lap times in Sprint.

I will maintain leaderboards for every track, and also a total leaderboard where participants will receive points based on their positions on the track leaderboards.

[b]Posting lap times[/b]
Everyone is welcome to post their lap times when they set a new personal record for at track.

You are encouraged to provide a video or screenshot in the post, but it is not necessary.

Please include track name, lap time and vehicle name in the post. It makes it a lot easier for me to maintain the leaderboards.
If the run was not made recently, you can also include the date of when it was made.

[b]Total leaderboard[/b]
The total leaderboard points are the sum of the points given in each track leaderboard. The formula for calculating points for each track is 21 - position. For example, position 1 will give 21 points - 1 = 20 points. Everyone on a track leaderboard will always receive at least 1 point.

[div style=""overflow:auto;""][table style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"Pos.".GetTdHeaderCell()}
{"Points".GetTdHeaderCell()}
{"Name".GetTdHeaderCell()}
{"No of tracks".GetTdHeaderCell()}
{"Avg points/track".GetTdHeaderCell()}
[/tr]
[/thead]
{string.Join('\n', result.Select((x, i) => GetSprintTotalLeaderboardTableRow(x, x.Position, i)))}

[/table][/div]
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
                Track = tracks.First(x =>
                    x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName), cols[races.TrackColumn - 1])),
                Vehicle = ClosestMatch(vehicles, cols[races.VehicleColumn - 1]),
                RunDate = races.RunDateColumn != 0
                    ? DateTime.ParseExact(cols[races.RunDateColumn - 1].Trim(), races.RunDateFormat, null)
                    : null,
                Member = await masterDataService.GetMemberAsync(races.MemberId),
                PostUrl = races.PostUrl
            };


            if (races.MediaLinkColumn != 0)
            {
                gd.MediaLink = cols[races.MediaLinkColumn - 1];
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
        var byTrack = await GetSprintLeaderboardByTrackAsync(races);
        var byMember = await GetSprintLeaderboardByMemberAsync(races);

        var report = new SprintReportDTO
        {
            LeaderBoardByTrack = byTrack,
            LeaderBoardByMember = byMember,
            TotalLeaderBoard = await GetSprintTotalLeaderboardTableAsync(races)
        };
        report.LeaderBoardByMemberHtml = report.LeaderBoardByMember.ToHtml();
        report.LeaderBoardByTrackHtml = report.LeaderBoardByTrack.ToHtml();
        report.TotalLeaderBoardHtml = report.TotalLeaderBoard.ToHtml();
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

        var timeIndex = 1;
        var trackIndex = 0;

        if (char.IsDigit(templateRows.First().First()))
        {
            timeIndex = 0;
            trackIndex = 1;
        }

        r.Time = templateRows[timeIndex].Trim().FromTimestringToInt();
        r.Track = tracks.FirstOrDefault(x =>
            x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName), templateRows[trackIndex]));

        templateRows.RemoveRange(0, 2);

        var videoLinkIndex = templateRows.FindIndex(x => x.StartsWith("http"));
        if (videoLinkIndex != -1)
        {
            r.MediaLink = templateRows[videoLinkIndex].Trim();
            templateRows.RemoveAt(videoLinkIndex);
        }

        r.Vehicle = ClosestMatch(vehicles, templateRows[0]);

        return r;
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

    private DateTime? GetDate()
    {
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
[/tr]
[/thead]
{string.Join('\n', races.Select((x, i) => GetSprintLeaderboardTableRowByTrack(x, i)))}

[/table][/div]
[/spoiler]";
    }

    private string ClosestMatch(IEnumerable<string> m, string s)
    {
        //var distance = int.MaxValue;//
        var distance = double.MaxValue;
        var result = "";
        var lcs = new LongestCommonSubsequence();

        foreach (var t in m)
        {
            var d = lcs.Distance(t, s);
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
    }
}