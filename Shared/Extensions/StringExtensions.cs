namespace Shared.Extensions;

public static class StringExtensions
{
    public static int FromTimestringToInt(this string t)
    {
        var time = new string([.. t.Where(char.IsDigit)]);

        // Special case with minute and second without leading 0 (ex: 1:5:564)
        if (time.Length == 5 && time.StartsWith("1"))
        {
            var ms = int.Parse(time.Substring(time.Length - 3));
            var s = int.Parse(time.Substring(1, 2)) * 1000;
            var m = 60000;
            return m + s + ms;
        }

        if (time.Length == 5 || time.Length == 6 || (time.Length == 7 && time.StartsWith("0")))
        {
            var ms = int.Parse(time.Substring(time.Length - 3));
            var s = int.Parse(time.Substring(time.Length - 5, 2)) * 1000;
            var m = time.Length > 5 ? int.Parse(time.Substring(0, time.Length - 5)) * 60000 : 0;
            return m + s + ms;
        }

        throw new Exception("Unable to parse time string");
    }

    public static string GetTdCell(this string text, string style = "", bool spanText = true,
        bool emptyCellIfMissing = true)
    {
        if (string.IsNullOrEmpty(text) && !emptyCellIfMissing)
            return "";

        if (spanText)
            return $"[td]{text}[/td]";
        return $"[td]{text}[/td]";
    }

    public static string GetTdHeaderCell(this string text, string style = "", bool spanText = true)
    {
        style += ";border: 1px solid";

        if (spanText)
            return $"[th style=\"padding:1px;white-space: nowrap;{style}\"]{text}[/th]";
        return $"[th style=\"padding:1px;{style}\"]{text}[/th]";
    }

    public static string ToHtml(this string bbCode)
    {
        return bbCode.Replace("[", "<").Replace("]", ">");
    }
}