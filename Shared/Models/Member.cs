using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Member : Item
{
    public required string MemberName { get; set; }
    public required string MemberDisplayName { get; set; }
    public bool Guest { get; set; } = false;
}