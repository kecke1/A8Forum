namespace Shared.Dto;

public class GauntletLeaderboardDto
{
    public required string id { get; set; }
    public DateTime CreatedDate { get; set; }
    public required IList<GauntletLeaderboardRowDto> Leaderboard { get; set; }
}