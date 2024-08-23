namespace Shared.Dto
{
    public class GiftLinkDTO
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public GiftLinkProviderDTO GiftLinkProvider { get; set; }
        public MemberDTO Member { get; set; }
        public bool Deleted { get; set; }
        public DateTime Idate { get; set; }
        public DateTime Month { get; set; }
        public string? Notes { get; set; }
    }
}