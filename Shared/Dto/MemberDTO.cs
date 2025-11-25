namespace Shared.Dto;

public class MemberDTO
{
    public required string Id { get; set; }
    public required string MemberName { get; set; }
    public required string MemberDisplayName { get; set; }
    public bool Guest { get; set; }
    public int? VipLevel { get; set; }
    public bool Deleted { get; set; }
    public bool Hidden { get; set; }
    public IEnumerable<string> RacingNames { get; set; } = [];
    public IEnumerable<string> FormerRacingNames { get; set; } = [];
}