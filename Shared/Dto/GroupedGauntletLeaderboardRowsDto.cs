namespace Shared.Dto;

public class GroupedGauntletLeaderboardRowsDto
{
    public string Name { get; set; }
    public IOrderedEnumerable<GauntletLeaderboardRowDto> Rows { get; set; }
}
