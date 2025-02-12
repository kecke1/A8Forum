using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Track : Item
{
    public required string TrackName { get; set; }
    public required bool Sprint { get; set; } = false;
}