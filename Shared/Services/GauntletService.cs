using F23.StringSimilarity;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Dto;
using Shared.Extensions;
using Shared.Mappers;
using Shared.Models;
using Shared.Options;

namespace Shared.Services;

public class GauntletService : IGauntletService
{
    private readonly IRepository<GauntletRun> _gauntletRunRepository;
    private readonly IMasterDataService _masterDataService;
    private readonly string _vehiclesBaseUrl;

    public GauntletService(IRepository<GauntletRun> gauntletRunRepository,
        IMasterDataService masterDataService,
         IOptions<A8Options> options)
    {
        _gauntletRunRepository = gauntletRunRepository;
        _masterDataService = masterDataService;
        _vehiclesBaseUrl = options.Value.VehiclesBaseUrl;
    }

    public async Task AddGauntletRunAsync(GauntletRunDTO r)
    {
        await _gauntletRunRepository.CreateAsync(r.ToGauntletRunEntity());
    }

    public async Task DeleteGauntletRunAsync(string gauntletRunId)
    {
        var r = await GetGauntletRunAsync(gauntletRunId);
        r.Deleted = true;
        await _gauntletRunRepository.UpdateAsync(r.ToGauntletRunEntity());
    }

    public async Task<IOrderedEnumerable<GauntletLeaderboardRowDto>> GetGauntletLeaderboardRowsAsync()
    {
        var races = (await GetGauntletRunsAsync())
            .Where(x => !x.Deleted);

        var created = DateTime.Now;
        var q = races
            .GroupBy(x => new { MemberId = x.Member.Id, TrackId = x.Track.Id }, y => y)
            .Select(x =>
            {
                var orderedRaces = x.OrderBy(y => y.Time);
                var bestRace = orderedRaces.First();

                var c = new GauntletLeaderboardRowDto
                {
                    MemberName = bestRace.Member.MemberName,
                    MemberId = x.Key.MemberId,
                    MemberDisplayName = bestRace.Member.MemberDisplayName,
                    PostUrl = bestRace.PostUrl ?? "",
                    LapTimeVerified = bestRace.LapTimeVerified,
                    MediaLink = bestRace.MediaLink ?? "",
                    RunDate = bestRace.RunDate,
                    Time = bestRace.Time,
                    TimeString = bestRace.Time.ToTimeString(),
                    TrackId = x.Key.TrackId,
                    TrackName = bestRace.Track.TrackName,
                    VehicleName1 = bestRace.Vehicle1.ShortName,
                    VehicleName2 = bestRace.Vehicle2.ShortName,
                    VehicleName3 = bestRace.Vehicle3.ShortName,
                    VehicleName4 = bestRace.Vehicle4?.ShortName,
                    VehicleName5 = bestRace.Vehicle5?.ShortName,
                    VehicleUrl1 = string.IsNullOrEmpty(bestRace.Vehicle1.Url) ? "" : _vehiclesBaseUrl + bestRace.Vehicle1.Url,
                    VehicleUrl2 = string.IsNullOrEmpty(bestRace.Vehicle2.Url) ? "" : _vehiclesBaseUrl + bestRace.Vehicle2.Url,
                    VehicleUrl3 = string.IsNullOrEmpty(bestRace.Vehicle3.Url) ? "" : _vehiclesBaseUrl + bestRace.Vehicle3.Url,
                    VehicleUrl4 = string.IsNullOrEmpty(bestRace.Vehicle4?.Url) ? "" : _vehiclesBaseUrl + bestRace.Vehicle4.Url,
                    VehicleUrl5 = string.IsNullOrEmpty(bestRace.Vehicle5?.Url) ? "" : _vehiclesBaseUrl + bestRace.Vehicle5.Url,
                    Runs = orderedRaces
                };

                return c;
            })
            .ToArray()
            .OrderBy(x => x.TrackName)
            .ThenBy(x => x.Time);

        var withPositions = new List<GauntletLeaderboardRowDto>();
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

    public async Task<string> GetGauntletLeaderboardByMemberAsync(IOrderedEnumerable<GauntletLeaderboardRowDto> races)
    {
        var tables = new List<string>();
        foreach (var track in races.GroupBy(x => x.MemberDisplayName).OrderBy(x => x.Key))
        {
            tables.Add(GetGauntletLeaderboardTableByMember(track.Select(x => x).OrderBy(x => x.TrackName)));
        }
        var output = @$"[b]Best lap times[/b]

        {string.Join("\n", tables)}";
        return output;
    }

    public async Task<string> GetGauntletLeaderboardByTrackAsync(IOrderedEnumerable<GauntletLeaderboardRowDto> races)
    {
        var tables = new List<string>();
        foreach (var track in races.GroupBy(x => x.TrackName).OrderBy(x => x.Key))
        {
            tables.Add(GetGauntletLeaderboardTableByTrack(track.Select(x => x)
                .OrderBy(x => x.Position)
                .ThenByDescending(x => x.RunDate ?? DateTime.MinValue)));
        }

        var output = @$"[b]Leaderboards by track[/b]

{string.Join("\n", tables)}";

        return output;
    }

    public async Task<GauntletRunDTO> GetGauntletRunAsync(string gauntletRunId)
    {
        var r = await _gauntletRunRepository.GetAsync(gauntletRunId);
        var member = await _masterDataService.GetMemberAsync(r.MemberId);
        var track = await _masterDataService.GetTrackAsync(r.TrackId);
        var vehicles = (await _masterDataService.GetVehiclesAsync()).ToArray();

        return r.ToDto(track,
            vehicles.Single(y => y.Id == r.Vehicle1Id),
            vehicles.Single(y => y.Id == r.Vehicle2Id),
            vehicles.Single(y => y.Id == r.Vehicle3Id),
            vehicles.SingleOrDefault(y => y.Id == r.Vehicle4Id),
            vehicles.SingleOrDefault(y => y.Id == r.Vehicle5Id),
            member);
    }

    public async Task<IEnumerable<GauntletRunDTO>> GetGauntletRunsAsync()
    {
        var vehicles = (await _masterDataService.GetVehiclesAsync()).ToArray();
        var tracks = (await _masterDataService.GetTracksAsync()).ToArray();
        var members = (await _masterDataService.GetMembersAsync()).ToArray();
        var r = (await _gauntletRunRepository.GetAsync(x => true)).ToArray();

        return r.Select(x => x
        .ToDto(tracks
        .Single(y => y.Id == x.TrackId),
        vehicles.Single(y => y.Id == x.Vehicle1Id),
        vehicles.Single(y => y.Id == x.Vehicle2Id),
        vehicles.Single(y => y.Id == x.Vehicle3Id),
        vehicles.SingleOrDefault(y => y.Id == x.Vehicle4Id),
        vehicles.SingleOrDefault(y => y.Id == x.Vehicle5Id),
        members.Single(y => y.Id == x.MemberId)))
        .ToArray();
    }

    private class GauntletLeaderboardResultDto
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
        public int NumberOfTracks { get; set; }
        public double AvgPoints { get; set; }
    }

    private string GetGautletTotalLeaderboardTableRow(GauntletLeaderboardResultDto g, int position, int index)
    {
        return
            @$"[tr {(index % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}]
{($"{position}.".GetTdCell("padding:1px;padding-left:7px", true))}
{($"{g.Points}".GetTdCell("padding:1px;padding-left:7px", true))}
{($"@{g.Name}".GetTdCell("padding:1px;padding-left:7px", true))}
{($"{g.NumberOfTracks}".GetTdCell("padding:1px;padding-left:7px", true))}
{($"{Math.Round(g.AvgPoints, 1)}".GetTdCell("padding:1px;padding-left:7px", true))}
[/tr]";
    }

    public async Task<string> GetGauntletTotalLeaderboardTableAsync(IEnumerable<GauntletLeaderboardRowDto> races)
    {
        var p = 1;
        var result = races.GroupBy(x => x.MemberId)
            .Select(x => new GauntletLeaderboardResultDto { Name = x.First().MemberName, Points = x.Sum(y => y.PositionPoints), Position = 0, NumberOfTracks = x.Count(), AvgPoints = x.Count() > 0 ? x.Sum(y => y.PositionPoints) / Convert.ToDouble(x.Count()) : 0 })
            .GroupBy(x => x.Points, y => y)
            .OrderByDescending(x => x.Key)
            .SelectMany((x, i) =>
            {
                foreach (var r in x)
                {
                    r.Position = p;
                }

                p += x.Count();
                return x;
            })
            .OrderBy(x => x.Position);

        var table = $@"This is the forum challenge for Gauntlet, it will go on as long as there is Gauntlet mode in A8.

The purpose is, in addition to a friendly competition between forum members, to keep a record of useful car combinations.

It will hopefully also be a starting point for discussing how to improve lap times in Gauntlet.

I will maintain leaderboards for every track, and also a total leaderboard where participants will receive points based on their positions on the track leaderboards.

[b]Posting lap times[/b]
Everyone is welcome to post their lap times when they set a new personal record for at track.

You are encouraged to provide a video or screenshot in the post. If you post a screenshot, it should include the track name and time if it's possible. The time and track are visible when you press the Defence button in Gauntlet.
The first column in the leaderbords will indicate if the lap time has been verified with a video or a screenshot that shows track name and lap time.

It is still ok to post lap times without verification. I think everyone here trust each other not to make up lap times. But it has happened in regular forum challenge on serveral occations that someone has posted lap times with wrong track by mistake.

Please wait to post your defense lap until the end of the week or when you are sure that you will not improve your time later in the same week. If you want to discuss your current weeks gauntlet defence, you can do that in 'Gauntlet General Discussion'

If your post doesn't has a screenshot where track name is visible, you should write down the name of the track.

It is also very helpful if you write your time and which vehicles you used.

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
{string.Join('\n', result.Select((x, i) => GetGautletTotalLeaderboardTableRow(x, x.Position, i)))}

[/table][/div]
";

        return table;
    }

    public async Task ImportGauntletRunsAsync(GauntletImportDTO races)
    {
        var tracks = (await _masterDataService.GetTracksAsync()).ToArray();
        var vehicles = (await _masterDataService.GetVehiclesAsync()).Where(x => x.MaxRank > 1850).ToArray();

        var toCreate = new List<GauntletRunDTO>();

        foreach (var row in races.ImportedText.Split('\n'))
        {
            if (string.IsNullOrEmpty(row))
                continue;

            var gd = new GauntletRunDTO();
            gd.Idate = DateTime.Now;

            var cols = row.Trim().Split(string.IsNullOrEmpty(races.Seperator) ? "\t" : races.Seperator).Select(x => x.Trim()).ToArray();

            gd.Time = cols[races.TimeColumn - 1].FromTimestringToInt();

            gd.Track = tracks.First(x => x.TrackName == ClosestMatch(tracks.Select(x => x.TrackName), cols[races.TrackColumn - 1]));

            var v = cols[races.VehiclesColumn - 1].Split(races.VehiclesSeperator);

            gd.Vehicle1 = ClosestMatch(vehicles, v[0]);
            gd.Vehicle2 = ClosestMatch(vehicles, v[1]);
            gd.Vehicle3 = ClosestMatch(vehicles, v[2]);

            if (v.Length > 3)
            {
                gd.Vehicle4 = ClosestMatch(vehicles, v[3]);
            }

            if (v.Length > 4)
            {
                gd.Vehicle5 = ClosestMatch(vehicles, v[4]);
            }

            if (races.RunDateColumn != 0)
            {
                gd.RunDate = DateTime.ParseExact(cols[races.RunDateColumn - 1].Trim(), races.RunDateFormat, null);
            }

            if (races.VerifiedColumn != 0)
            {
                if (cols[races.VerifiedColumn - 1] == "1")
                {
                    gd.LapTimeVerified = true;
                }
            }

            if (races.MediaLinkColumn != 0)
            {
                gd.MediaLink = cols[races.MediaLinkColumn - 1];
                gd.LapTimeVerified = true;
            }

            gd.Member = await _masterDataService.GetMemberAsync(races.MemberId);
            gd.PostUrl = races.PostUrl;

            toCreate.Add(gd);
        }

        foreach (var r in toCreate)
        {
            await _gauntletRunRepository.CreateAsync(r.ToGauntletRunEntity());
        }
    }

    public async Task UpdateGauntletRunAsync(GauntletRunDTO r)
    {
        await _gauntletRunRepository.UpdateAsync(r.ToGauntletRunEntity());
    }

    private string GetGauntletLeaderboardTableByMember(IOrderedEnumerable<GauntletLeaderboardRowDto> races)
    {
        var numberOfVehicles = GetNumberOfUsedGauntletVehicles(races);

        var output = @$"´[b]@{races.First().MemberName}[/b][spoiler][div style=""overflow:auto;""][table style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"✅".GetTdHeaderCell("", false)}
{"Track".GetTdHeaderCell()}
{"Time".GetTdHeaderCell()}
{"Date".GetTdHeaderCell()}
{"Video".GetTdHeaderCell("", false)}
{"Vehicle 1".GetTdHeaderCell()}
{"Vehicle 2".GetTdHeaderCell()}
{"Vehicle 3".GetTdHeaderCell()}
{(numberOfVehicles > 3 ? "Vehicle 4".GetTdHeaderCell() : "")}
{(numberOfVehicles > 4 ? "Vehicle 5".GetTdHeaderCell() : "")}
[/tr]
[/thead]
{string.Join('\n', races.Select((x, i) => GetGauntletLeaderboardTableRowByMember(x, i, numberOfVehicles)))}

[/table][/div]
[/spoiler]";

        return output;
    }

    private int GetNumberOfUsedGauntletVehicles(IOrderedEnumerable<GauntletLeaderboardRowDto> races)
    {
        return races.Any(x => !string.IsNullOrEmpty(x.VehicleName5)) ? 5 : races.Any(x => !string.IsNullOrEmpty(x.VehicleName4)) ? 4 : 3;
    }

    private string GetGauntletLeaderboardTableRowByMember(GauntletLeaderboardRowDto race, int i, int numberOfVehicles)
    {
        return $@"[tr {(i % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}]
{GetTdCellWithBorder(race.LapTimeVerified ? "✅" : "", false)}
{GetTdCellWithBorder(race.TrackName, true)}
{GetTdCellWithBorder(race.TimeString, true)}
{GetTdCellWithBorder((race.RunDate.HasValue ? race.RunDate.Value.ToString("dd.MM.yyyy") : ""), true)}
{GetTdCellWithBorder(!string.IsNullOrEmpty(race.MediaLink) ? GetLinkOrText(race.MediaLink, "🎦") : "", false)}
{GetGauntletTableVehicleCell(race.VehicleUrl1, race.VehicleName1, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl2, race.VehicleName2, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl3, race.VehicleName3, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl4, race.VehicleName4, numberOfVehicles > 3)}
{GetGauntletTableVehicleCell(race.VehicleUrl5, race.VehicleName5, numberOfVehicles > 4)}
[/tr]";
    }

    private string GetTdCellWithBorder(string text = "", bool spanText = true, bool emptyCellIfMissing = true)
    {
        return text.GetTdCell("border: 1px solid", spanText, emptyCellIfMissing);
    }

    private string GetLinkOrText(string linkUrl, string text)
    {
        if (string.IsNullOrEmpty(linkUrl))
            return text;
        return $"[a href=\"{linkUrl}\"]{text}[/a]";
    }

    private string GetGauntletTableVehicleCell(string url, string name, bool emptyCellIfMissing)
    {
        return GetTdCellWithBorder(name, true, emptyCellIfMissing);
    }

    private string GetGauntletLeaderboardTableRowByTrack(GauntletLeaderboardRowDto race, int i, int numberOfVehicles)
    {
        return $@"[tr {(i % 2 == 1 ? "style='background-color: #f3f3f3'" : "")}]

{GetTdCellWithBorder(race.LapTimeVerified ? "✅" : "", false)}
{GetTdCellWithBorder(($"{race.Position}."), false)}
{GetTdCellWithBorder(($"@{race.MemberName}"), true)}
{GetTdCellWithBorder(race.TimeString, true)}
{GetTdCellWithBorder(race.RunDate.HasValue ? race.RunDate.Value.ToString("dd.MM.yyyy") : "", race.RunDate.HasValue)}
{GetTdCellWithBorder(!string.IsNullOrEmpty(race.MediaLink) ? GetLinkOrText(race.MediaLink, "🎦") : "", false)}
{GetGauntletTableVehicleCell(race.VehicleUrl1, race.VehicleName1, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl2, race.VehicleName2, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl3, race.VehicleName3, true)}
{GetGauntletTableVehicleCell(race.VehicleUrl4, race.VehicleName4, numberOfVehicles > 3)}
{GetGauntletTableVehicleCell(race.VehicleUrl5, race.VehicleName5, numberOfVehicles > 4)}
[/tr]";
    }

    private string GetGauntletLeaderboardTableByTrack(IOrderedEnumerable<GauntletLeaderboardRowDto> races)
    {
        var numberOfVehicles = GetNumberOfUsedGauntletVehicles(races);

        return @$"[b]{races.First().TrackName}[/b][spoiler][div style=""overflow:auto;""][table style='border-collapse: collapse;font-family: arial, sans-serif;']
[thead style='background-color: black; color:white; font-weight:bold;']
[tr]
{"✅".GetTdHeaderCell("", false)}
{"Pos.".GetTdHeaderCell()}
{"Name".GetTdHeaderCell()}
{"Time".GetTdHeaderCell()}
{"Date".GetTdHeaderCell()}
{"Video".GetTdHeaderCell()}
{"Vehicle 1".GetTdHeaderCell()}
{"Vehicle 2".GetTdHeaderCell()}
{"Vehicle 3".GetTdHeaderCell()}
{(numberOfVehicles > 3 ? "Vehicle 4".GetTdHeaderCell() : "")}
{(numberOfVehicles > 4 ? "Vehicle 5".GetTdHeaderCell() : "")}
[/tr]
[/thead]
{string.Join('\n', races.Select((x, i) => GetGauntletLeaderboardTableRowByTrack(x, i, numberOfVehicles)))}

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

    private VehicleDTO ClosestMatch(IEnumerable<VehicleDTO> vehicles, string s)
    {
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
                {
                    shortest = kwDistance;
                }
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

    public async Task<GauntletReportDTO> GetGauntletReportAsync()
    {
        var report = new GauntletReportDTO();

        var races = await GetGauntletLeaderboardRowsAsync();

        var byTrack = await GetGauntletLeaderboardByTrackAsync(races);
        report.LeaderBoardByTrack = byTrack;
        var byMember = await GetGauntletLeaderboardByMemberAsync(races);

        report.LeaderBoardByMember = byMember;
        report.TotalLeaderBoard = await GetGauntletTotalLeaderboardTableAsync(races);
        report.LeaderBoardByMemberHtml = report.LeaderBoardByMember.ToHtml();
        report.LeaderBoardByTrackHtml = report.LeaderBoardByTrack.ToHtml();
        report.TotalLeaderBoardHtml = report.TotalLeaderBoard.ToHtml();
        report.LeaderBoardJson = JsonConvert.SerializeObject(races);
        return report;
    }
}