using Shared.Dto;

namespace Shared.Mappers
{
    internal static class GiftLinkDTOMapper
    {
        public static GiftLinkDTO ToDto(this Shared.Models.GiftLink giftlink, GiftLinkProviderDTO p, MemberDTO m)
        {
            return new GiftLinkDTO
            {
                Id = giftlink.Id,
                Url = giftlink.Url,
                GiftLinkProvider = p,
                Member = m,
                Deleted = giftlink.Deleted,
                Idate = giftlink.Idate,
                Month = giftlink.Month,
                Notes = giftlink.Notes
            };
        }

        public static Shared.Models.GiftLink ToGiftLinkEntity(this GiftLinkDTO model)
        {
            var g = new Shared.Models.GiftLink
            {
                Url = model.Url,
                GiftLinkProviderId = model.GiftLinkProvider.Id ?? throw new NullReferenceException(),
                MemberId = model.Member.Id ?? throw new NullReferenceException(),
                Deleted = model.Deleted,
                Idate = model.Idate,
                Month = model.Month,
                Notes = model.Notes
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                g.Id = model.Id;
            }
            return g;
        }
    }
}