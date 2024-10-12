namespace A8Forum.Extensions;

public static class StringExtensions
{
    public static string CheckAndFormatUrl(this string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }

        if(!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
        {
            url = $"https://{url}";
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return url;
        }

        return "";
    }
}
