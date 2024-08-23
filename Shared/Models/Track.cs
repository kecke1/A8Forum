using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Track : Item
{
    public string TrackName { get; set; }
}