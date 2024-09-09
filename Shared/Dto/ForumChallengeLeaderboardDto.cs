namespace Shared.Dto;

public class ForumChallengeLeaderboardDto
{
    public required string MemberId { get; set; }
    public required string MemberName { get; set; }
    public required string MemberDisplayName { get; set; }
    public int Position { get; set; }
    public int SeriesPoints { get; set; }
    public required string TimeString { get; set; }
    public int Time { get; set; }
    public required string PostUrl { get; set; }
    public required string ForumChallengeId { get; set; }
    public DateTime ChallengeEndDate { get; set; }
    public required string VehicleName { get; set; }
    public required string VehicleUrl { get; set; }
    public required string ForumChallengeUrl { get; set; }
    public required string ForumChallengeTitleHtml { get; set; }

    public IOrderedEnumerable<ForumChallengeRunDTO> Runs { get; set; } =
        Enumerable.Empty<ForumChallengeRunDTO>().OrderBy(x => x);
}