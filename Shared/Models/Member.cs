using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Member : Item
{
    public string MemberName { get; set; }
    public string MemberDisplayName { get; set; }
    public bool Guest { get; set; } = false;
}