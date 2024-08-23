namespace A8Forum.Extensions;

public static class IntExtensions
{
    public static string ToTimeString(this int t)
    {
        var ts = TimeSpan.FromMilliseconds(t);
        return $"{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds:000}";
    }
}