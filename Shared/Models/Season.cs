using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Season : Item
{
    public required string SeasonName { get; set; }
}