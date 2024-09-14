namespace A8Forum.Extensions;

public static class StringExtensions
{
    public static string CheckAndFormatUrl(this string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }

        var u = url.ToLower();

        if(!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            u = $"https://{url}";
        }

        if (Uri.TryCreate(u, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return u;
        }

        return "";
    }
}
