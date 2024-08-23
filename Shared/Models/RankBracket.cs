using Microsoft.Azure.CosmosRepository;
using Shared.Enums;

namespace Shared.Models;

public class RankBracket : Item
{
    public int MinRank { get; set; }
    public int MaxRank { get; set; }
    public ClassEnum Class { get; set; }
}