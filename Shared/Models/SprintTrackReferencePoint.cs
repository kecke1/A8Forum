using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;
public class SprintTrackReferencePoint : Item
{
    public required string TrackId { get; set; }
    public DateTime Date { get; set; }
}
