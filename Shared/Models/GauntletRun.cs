using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class GauntletRun : Item
{
    public int Time { get; set; }
    public DateTime Idate { get; set; } = DateTime.Now;
    public bool Deleted { get; set; } = false;
    public string TrackId { get; set; }
    public string Vehicle1Id { get; set; }
    public string Vehicle2Id { get; set; }
    public string Vehicle3Id { get; set; }
    public string? Vehicle4Id { get; set; }
    public string? Vehicle5Id { get; set; }
    public string MemberId { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
    public bool LapTimeVerified { get; set; } = false;
}