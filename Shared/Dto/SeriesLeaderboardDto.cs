namespace Shared.Dto;

public class SeriesLeaderboardDto
{
    public required string MemberId { get; set; }
    public string? MemberName { get; set; }
    public required string MemberDisplayName { get; set; }
    public int Position { get; set; }
    public int Points { get; set; }
    public int NumberOfRuns { get; set; }

    public required string LatestForumChallengeId { get; set; }
    public DateTime LatestChallengeEndDate { get; set; }
    public int LatestChallengeLapTime { get; set; }
    public required string LatestChallengeTimeString { get; set; }
}