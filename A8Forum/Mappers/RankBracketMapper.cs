using Shared.Dto;

namespace A8Forum.Mappers;

internal static class RankBracketMapper
{
    public static RankBracketDTO ToDto(this ViewModels.RankBracketViewModel rankBracket)
    {
        return new RankBracketDTO
        {
            Id = rankBracket.RankBracketId,
            Class = rankBracket.Class,
            MaxRank = rankBracket.MaxRank,
            MinRank = rankBracket.MinRank,
        };
    }

    public static ViewModels.RankBracketViewModel ToRankBracketViewModel(this RankBracketDTO model)
    {
        return new ViewModels.RankBracketViewModel
        {
            RankBracketId = model.Id,
            Class = model.Class,
            MinRank = model.MinRank,
            MaxRank = model.MaxRank
        };
    }
}