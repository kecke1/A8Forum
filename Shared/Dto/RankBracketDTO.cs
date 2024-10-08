using Shared.Enums;

namespace Shared.Dto;

public class RankBracketDTO
{
    public required string Id { get; set; }
    public int MinRank { get; set; }
    public int MaxRank { get; set; }
    public ClassEnum Class { get; set; }
}