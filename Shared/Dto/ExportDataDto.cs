using Shared.Models;

namespace Shared.Dto;

public class ExportDataDto
{
    public IEnumerable<Season> Seasons { get; set; }
    public IEnumerable<ForumChallenge> ForumChallenges { get; set; }
    public IEnumerable<Track> Tracks { get; set; }
    public IEnumerable<RankBracket> RankBrackets { get; set; }
    public IEnumerable<ForumChallengeRun> ForumChallengeRuns { get; set; }
    public IEnumerable<Series> Series { get; set; }
    public IEnumerable<GauntletRun> GautletRuns { get; set; }
    public IEnumerable<Vehicle> Vehicles { get; set; }
    public IEnumerable<Member> Members { get; set; }
    public IEnumerable<CareerRace> CareerRaces { get; set; }
}