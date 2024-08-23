namespace A8Forum.ViewModels;

public class GiftLinkProvider
{
    public string? GiftLinkProviderId { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public bool Deleted { get; set; } = false;
    public bool Hide { get; set; } = false;
}