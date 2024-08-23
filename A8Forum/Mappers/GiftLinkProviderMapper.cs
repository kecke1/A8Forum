using Shared.Dto;

namespace A8Forum.Mappers;

internal static class GiftLinkProviderMapper
{
    public static GiftLinkProviderDTO ToDto(this ViewModels.GiftLinkProvider g)
    {
        return new GiftLinkProviderDTO
        {
            Id = g.GiftLinkProviderId,
            Deleted = g.Deleted,
            Hide = g.Hide,
            Name = g.Name,
            Url = g.Url
        };
    }

    public static ViewModels.GiftLinkProvider ToGiftLinkProviderViewModel(this GiftLinkProviderDTO model)
    {
        return new ViewModels.GiftLinkProvider
        {
            GiftLinkProviderId = model.Id,
            Deleted = model.Deleted,
            Hide = model.Hide,
            Name = model.Name,
            Url = model.Url
        };
    }
}