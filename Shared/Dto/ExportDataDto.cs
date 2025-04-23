using Shared.Models;

namespace Shared.Dto;

public class ExportDataDto
{
    public required IEnumerable<Season> Seasons { get; set; }
    public required IEnumerable<ForumChallenge> ForumChallenges { get; set; }
    public required IEnumerable<Track> Tracks { get; set; }
    public required IEnumerable<RankBracket> RankBrackets { get; set; }
    public required IEnumerable<ForumChallengeRun> ForumChallengeRuns { get; set; }
    public required IEnumerable<Series> Series { get; set; }
    public required IEnumerable<GauntletRun> GautletRuns { get; set; }
    public required IEnumerable<Vehicle> Vehicles { get; set; }
    public required IEnumerable<Member> Members { get; set; }
    public required IEnumerable<CareerRace> CareerRaces { get; set; }
    public required IEnumerable<SprintRun> SprintRuns { get; set; }
    public required SprintTrackReferencePoint SprintTrackReferencePoint { get; set; }
}