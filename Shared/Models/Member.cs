using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Member : Item
{
    public required string MemberName { get; set; }
    public required string MemberDisplayName { get; set; }
    public bool Guest { get; set; } = false;
    public int? VipLevel { get; set; }
    public bool Deleted { get; set; }
    public bool Hidden { get; set; }
    public IEnumerable<string> RacingNames { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> FormerRacingNames { get; set; } = Enumerable.Empty<string>();
}