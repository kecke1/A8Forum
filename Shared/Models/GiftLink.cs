using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class GiftLink : Item
{
    public required string Url { get; set; }
    public required string GiftLinkProviderId { get; set; }
    public required string MemberId { get; set; }
    public bool Deleted { get; set; } = false;
    public DateTime Idate { get; set; } = DateTime.Now;
    public DateTime Month { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}