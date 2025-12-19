using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class GauntletRun : Item
{
    public int Time { get; set; }
    public DateTime Idate { get; set; } = DateTime.Now;
    public bool Deleted { get; set; } = false;
    public required string TrackId { get; set; }
    public required string Vehicle1Id { get; set; }
    public required string Vehicle2Id { get; set; }
    public required string Vehicle3Id { get; set; }
    public string? Vehicle4Id { get; set; }
    public string? Vehicle5Id { get; set; }
    public required string MemberId { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
    public bool LapTimeVerified { get; set; } = false;
    public bool A8Plus { get; set; } = false;
    public int? VipLevel { get; set; }
    public bool Shortcut { get; set; } = false;
    public bool Glitch { get; set; } = false;
}