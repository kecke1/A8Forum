namespace A8Forum.Extensions;

public static class StringExtensions
{
    public static string CheckAndFormatUrl(this string url)
    {
        if(string.IsNullOrEmpty(url) || url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
        {
            return url;
        }

        return $"https://{url}";
    }
}
