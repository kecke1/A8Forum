namespace Shared.Dto;

public class ForumChallengeRunDTO
{
    public required string Id { get; set; }
    public int Time { get; set; }
    public DateTime Idate { get; set; }
    public string? Post { get; set; }
    public bool Deleted { get; set; }
    public required ForumChallengeDTO ForumChallenge { get; set; }
    public required MemberDTO Member { get; set; }
    public VehicleDTO? Vehicle { get; set; }
}