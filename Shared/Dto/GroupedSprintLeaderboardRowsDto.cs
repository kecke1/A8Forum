namespace Shared.Dto;

public class GroupedSprintLeaderboardRowsDto
{
    public string Name { get; set; }
    public IOrderedEnumerable<SprintLeaderboardRowDto> Rows { get; set; }
}
