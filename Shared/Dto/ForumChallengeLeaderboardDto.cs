namespace Shared.Dto;

public class ForumChallengeLeaderboardDto
{
    public string MemberId { get; set; }
    public string MemberName { get; set; }
    public string MemberDisplayName { get; set; }
    public int Position { get; set; }
    public int SeriesPoints { get; set; }
    public string TimeString { get; set; }
    public int Time { get; set; }
    public string PostUrl { get; set; }
    public string ForumChallengeId { get; set; }
    public DateTime ChallengeEndDate { get; set; }
    public string VehicleName { get; set; }
    public string VehicleUrl { get; set; }
    public string ForumChallengeUrl { get; set; }
    public string ForumChallengeTitleHtml { get; set; }
    public IOrderedEnumerable<ForumChallengeRunDTO> Runs { get; set; } = Enumerable.Empty<ForumChallengeRunDTO>().OrderBy(x => x);
}