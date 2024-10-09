namespace Shared.Dto;

public class EditForumChallengeRunDTO
{
    public required string Id { get; set; }
    public int Time { get; set; }
    public DateTime Idate { get; set; }
    public string? Post { get; set; }
    public bool Deleted { get; set; }
    public required string ForumChallengeId { get; set; }
    public required string MemberId { get; set; }
    public required string VehicleId { get; set; }
}