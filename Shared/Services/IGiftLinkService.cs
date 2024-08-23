using Shared.Dto;

namespace Shared.Services;

public interface IGiftLinkService
{
    public Task<IEnumerable<GiftLinkDTO>> GetGiftLinksAsync();

    public Task<GiftLinkDTO> GetGiftLinkAsync(string giftLinkId);

    public Task UpdateGiftLinkAsync(GiftLinkDTO g);

    public Task AddGiftLinkAsync(GiftLinkDTO g);

    public Task DeleteGiftLinkAsync(string giftLinkId);

    public Task<IEnumerable<GiftLinkProviderDTO>> GetGiftLinkProvidersAsync();

    public Task<GiftLinkProviderDTO> GetGiftLinkProviderAsync(string giftLinkProviderId);

    public Task UpdateGiftLinkProviderAsync(GiftLinkProviderDTO g);

    public Task AddGiftLinkProviderAsync(GiftLinkProviderDTO g);

    public Task DeleteGiftLinkProviderAsync(string giftLinkProviderId);

    public string CreateGiftLinkList(IEnumerable<GiftLinkDTO> links);

    public IEnumerable<GiftLinkProviderWithDateDTO> GetActiveGiftLinkProviders(DateTime month,
        IEnumerable<GiftLinkProviderDTO> providers,
        IEnumerable<GiftLinkDTO> giftLinks);

    public string CreateGiftLinkProviderList(IEnumerable<GiftLinkProviderWithDateDTO> giftLinkProviders,
        DateTime month);
}