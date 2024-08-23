namespace Shared.Dto
{
    public class GiftLinkProviderDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Deleted { get; set; }
        public bool Hide { get; set; }
        public DateTime? LatestGiftLinkDate { get; set; }
    }
}