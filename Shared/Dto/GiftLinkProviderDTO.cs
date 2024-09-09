namespace Shared.Dto;

public class GiftLinkProviderDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool Deleted { get; set; }
    public bool Hide { get; set; }
    public DateTime? LatestGiftLinkDate { get; set; }
}