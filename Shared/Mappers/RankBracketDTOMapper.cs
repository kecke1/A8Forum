using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class RankBracketDTOMapper
{
    public static RankBracketDTO ToDto(this RankBracket rankbracket)
    {
        return new RankBracketDTO
        {
            Id = rankbracket.Id,
            MinRank = rankbracket.MinRank,
            MaxRank = rankbracket.MaxRank,
            Class = rankbracket.Class
        };
    }

    public static RankBracket ToRankBracketEntity(this RankBracketDTO model)
    {
        var r = new RankBracket
        {
            MinRank = model.MinRank,
            MaxRank = model.MaxRank,
            Class = model.Class
        };

        if (!string.IsNullOrEmpty(model.Id))
            r.Id = model.Id;

        return r;
    }
}