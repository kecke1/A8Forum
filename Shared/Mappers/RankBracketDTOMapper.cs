using Shared.Dto;

namespace Shared.Mappers
{
    internal static class RankBracketDTOMapper
    {
        public static RankBracketDTO ToDto(this Shared.Models.RankBracket rankbracket)
        {
            return new RankBracketDTO
            {
                Id = rankbracket.Id,
                MinRank = rankbracket.MinRank,
                MaxRank = rankbracket.MaxRank,
                Class = rankbracket.Class
            };
        }

        public static Shared.Models.RankBracket ToRankBracketEntity(this RankBracketDTO model)
        {
            var r = new Shared.Models.RankBracket
            {
                MinRank = model.MinRank,
                MaxRank = model.MaxRank,
                Class = model.Class
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                r.Id = model.Id;
            }

            return r;
        }
    }
}