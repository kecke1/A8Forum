using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class RankBracketMapper
{
    public static RankBracketDTO ToDto(this RankBracketViewModel rankBracket)
    {
        return new RankBracketDTO
        {
            Id = rankBracket.RankBracketId,
            Class = rankBracket.Class,
            MaxRank = rankBracket.MaxRank,
            MinRank = rankBracket.MinRank
        };
    }

    public static RankBracketViewModel ToRankBracketViewModel(this RankBracketDTO model)
    {
        return new RankBracketViewModel
        {
            RankBracketId = model.Id,
            Class = model.Class,
            MinRank = model.MinRank,
            MaxRank = model.MaxRank
        };
    }
}