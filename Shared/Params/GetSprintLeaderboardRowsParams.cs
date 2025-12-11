namespace Shared.Params;

public class GetSprintLeaderboardRowsParams
{
    public int MinVipLevel { get; set; } = 0;
    public int MaxVipLevel { get; set; } = 15;
    public DateTime? Date { get; set; }
    public bool IncludeFilteredOutVipMembers { get; set; } = false;
}
