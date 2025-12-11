namespace Shared.Params;

public class ClosestMatchParams
{
    public bool MatchFirstCharachter { get; set; } = false;
    public bool MatchLastCharachter { get; set; } = false;
    public bool MatchCase { get; set; } = false;
    public bool ContainsRev { get; set; } = false;
}
