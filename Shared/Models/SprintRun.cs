using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class SprintRun : Item
{
    public int Time { get; set; }
    public DateTime Idate { get; set; } = DateTime.Now;
    public bool Deleted { get; set; } = false;
    public required string TrackId { get; set; }
    public required string VehicleId { get; set; }
    public required string MemberId { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
}