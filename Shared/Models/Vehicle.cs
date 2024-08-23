using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Vehicle : Item
{
    public string Name { get; set; }
    public string ShortName { get; set; } = "";
    public string Keyword { get; set; } = "";
    public string? Url { get; set; }
    public int? MaxRank { get; set; }
}