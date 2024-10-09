using Shared.Dto;

namespace Shared.Services;

public interface IGauntletService
{
    public Task<IEnumerable<GauntletRunDTO>> GetGauntletRunsAsync();

    public Task<GauntletRunDTO> GetGauntletRunAsync(string gauntletRunId);

    public Task UpdateGauntletRunAsync(EditGauntletRunDTO r);

    public Task AddGauntletRunAsync(EditGauntletRunDTO r);

    public Task DeleteGauntletRunAsync(string gauntletRunId);

    public Task ImportGauntletRunsAsync(GauntletImportDTO races);

    public Task<IOrderedEnumerable<GauntletLeaderboardRowDto>> GetGauntletLeaderboardRowsAsync();

    public Task<string> GetGauntletLeaderboardByMemberAsync(IOrderedEnumerable<GauntletLeaderboardRowDto> races);

    public Task<string> GetGauntletLeaderboardByTrackAsync(IOrderedEnumerable<GauntletLeaderboardRowDto> races);

    public Task<string> GetGauntletTotalLeaderboardTableAsync(IEnumerable<GauntletLeaderboardRowDto> races);

    public Task<GauntletReportDTO> GetGauntletReportAsync();
}