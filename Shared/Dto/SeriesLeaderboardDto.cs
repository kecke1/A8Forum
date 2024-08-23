namespace Shared.Dto;

public class SeriesLeaderboardDto
{
    public string MemberId { get; set; }
    public string MemberName { get; set; }
    public string MemberDisplayName { get; set; }
    public int Position { get; set; }
    public int Points { get; set; }
    public int NumberOfRuns { get; set; }

    public string LatestForumChallengeId { get; set; }
    public DateTime LatestChallengeEndDate { get; set; }
    public int LatestChallengeLapTime { get; set; }
    public string LatestChallengeTimeString { get; set; }
}