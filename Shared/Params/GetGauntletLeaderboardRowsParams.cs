namespace Shared.Params;

public class GetGauntletLeaderboardRowsParams
{
    public int MinVipLevel { get; set; } = 0;
    public int MaxVipLevel { get; set; } = 15;
    public bool IncludeVerified { get; set; } = true;
    public bool IncludeUnverified { get; set; }
    public DateTime? Date { get; set; }
    public bool IncludeFilteredOutVipMembers { get; set; } = false;
}
