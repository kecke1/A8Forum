using Shared.Dto;
using Shared.Params;
using static Shared.Services.SprintService;

namespace Shared.Services;

public interface ISprintService
{
    public Task<IEnumerable<SprintRunDTO>> GetSprintRunsAsync();

    public Task<SprintRunDTO> GetSprintRunAsync(string sprintRunId);

    public Task UpdateSprintRunAsync(EditSprintRunDTO r);

    public Task AddSprintRunAsync(EditSprintRunDTO r);

    public Task DeleteSprintRunAsync(string sprintRunId);

    public Task ImportSprintRunsAsync(SprintImportDTO runs);

    public Task<IOrderedEnumerable<SprintLeaderboardRowDto>> GetSprintLeaderboardRowsAsync(GetSprintLeaderboardRowsParams p);

    public Task<string> GetSprintLeaderboardByMemberAsync(IOrderedEnumerable<SprintLeaderboardRowDto> runs);

    public Task<string> GetSprintLeaderboardByTrackAsync(IOrderedEnumerable<SprintLeaderboardRowDto> runs);

    public Task<string> GetSprintTotalLeaderboardTableAsync(IEnumerable<SprintLeaderboardRowDto> runs);

    public Task<string> GetSprintTotalLeaderboardPageAsync(IEnumerable<SprintLeaderboardRowDto> runs, IEnumerable<SprintLeaderboardRowDto> runsNoVip);

    public Task<SprintReportDTO> GetSprintReportAsync();

    public Task<SprintRunDTO> GetSprintRunFromTemplateAsync(string template, string postUrl);
    public Task UpsertSprintTrackReferencePointAsync(SprintTrackReferencePointDto r);
    public Task<SprintTrackReferencePointDto?> GetSprintTrackReferencePointAsync();
    public Task<SprintScheduleDTO> GetSprintScheduleAsync(DateTime startDate);

    public Task<IOrderedEnumerable<GroupedSprintLeaderboardRowsDto>> GetSprintLeaderboardByTrack(
        GetSprintLeaderboardRowsParams p);

    public Task<IOrderedEnumerable<GroupedSprintLeaderboardRowsDto>> GetSprintLeaderboardByMember(
        GetSprintLeaderboardRowsParams p);

    public Task<IOrderedEnumerable<SprintLeaderboardResultDto>> GetSprintTotalLeaderboardAsync(
        GetSprintLeaderboardRowsParams param);

}