using Shared.Dto;

namespace Shared.Mappers
{
    internal static class GiftLinkProviderDTOMapper
    {
        public static GiftLinkProviderDTO ToDto(this Shared.Models.GiftLinkProvider giftlinkprovider)
        {
            return new GiftLinkProviderDTO
            {
                Id = giftlinkprovider.Id,
                Name = giftlinkprovider.Name,
                Url = giftlinkprovider.Url,
                Deleted = giftlinkprovider.Deleted,
                Hide = giftlinkprovider.Hide
            };
        }

        public static Shared.Models.GiftLinkProvider ToGiftLinkProviderEntity(this GiftLinkProviderDTO model)
        {
            var g = new Shared.Models.GiftLinkProvider
            {
                Name = model.Name,
                Url = model.Url,
                Deleted = model.Deleted,
                Hide = model.Hide
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                g.Id = model.Id;
            }

            return g;
        }
    }
}