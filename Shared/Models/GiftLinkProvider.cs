using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class GiftLinkProvider : Item
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool Deleted { get; set; } = false;
    public bool Hide { get; set; }
}