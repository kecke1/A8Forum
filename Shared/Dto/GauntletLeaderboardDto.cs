namespace Shared.Dto;

public class GauntletLeaderboardDto
{
    public string id { get; set; }
    public DateTime CreatedDate { get; set; }
    public IList<GauntletLeaderboardRowDto> Leaderboard { get; set; }
}