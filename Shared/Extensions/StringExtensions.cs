using F23.StringSimilarity;
using Shared.Dto;
using Shared.Models;
using Shared.Params;

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

    public static string Match(this string input, IEnumerable<string> items, ClosestMatchParams p)
    {
        var distance = double.MaxValue;
        var result = "";
        var lcs = new LongestCommonSubsequence();

        foreach (var t in items
                     .Where(x => (!p.MatchFirstCharachter || x.StartsWith(input.First().ToString(), p.MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase))
                                 && (!p.MatchLastCharachter || x.EndsWith(input.Last().ToString(), p.MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase))
                                 && (!p.ContainsRev || input.Contains("rev", p.MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) == x.Contains("rev", p.MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase))))
        {
            var d = lcs.Distance(p.MatchCase ? t : t.ToLower(), p.MatchCase ? input : input.ToLower());
            if (d < distance)
            {
                distance = d;
                result = t;
            }
        }

        return result;
    }

    public static VehicleDTO Match(this string s, IEnumerable<VehicleDTO> vehicles)
    {

        if (s == null)
            return null;

        var sportline = false;
        var normalized = s.ToLower();
        if (s.Contains("sportline") || s.Contains(" sl"))
        {
            sportline = true;
            s = s.Replace(" sl", " sportline");
        }

        //var distance = int.MaxValue;//
        var distance = double.MaxValue;
        var result = vehicles.First();
        var lcs = new LongestCommonSubsequence();


        foreach (var v in vehicles.Where(x => !sportline || x.Name.ToLower().Contains("sportline")))
        {
            var shortest = lcs.Distance( v.ShortName.ToLower(), s.ToLower());

            if (!string.IsNullOrEmpty(v.Keyword))
            {
                foreach (var d in v.Keyword.Split(';').Where(x => !string.IsNullOrEmpty(x)))
                {
                    var kwDistance = lcs.Distance(d.Trim(), s);
                    if (kwDistance < shortest)
                        shortest = kwDistance;
                }
            }

            // var d = Fastenshtein.Levenshtein.Distance(t, s);
            if (shortest < distance)
            {
                distance = shortest;
                result = v;
            }
        }

        return result;
    }


}