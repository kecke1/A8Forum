using Shared.Dto;

namespace Shared.Services;

public interface IForumChallengeService
{
    public Task<IEnumerable<ForumChallengeDTO>> GetForumChallengesAsync(DateTime? fromDate = null,
        DateTime? toDate = null);

    public Task<ForumChallengeDTO> GetForumChallengeAsync(string forumChallengeId);

    public Task UpdateForumChallengeAsync(ForumChallengeDTO f);

    public Task AddForumChallengeAsync(ForumChallengeDTO f);

    public Task DeleteForumChallengeAsync(string forumChallengeId);

    public Task<IEnumerable<SeriesDTO>> GetSeriesAsync(DateTime? fromDate = null, DateTime? toDate = null);

    public Task<SeriesDTO> GetSeriesAsync(string seriesId);

    public Task UpdateSeriesAsync(SeriesDTO s);

    public Task AddSeriesAsync(SeriesDTO s);

    public Task DeleteSeriesAsync(string seriesId);

    public Task<IEnumerable<ForumChallengeRunDTO>> GetForumChallengeRunsAsync(string forumChallengeId);

    public Task<ForumChallengeRunDTO> GetForumChallengeRunAsync(string runId);

    public Task UpdateForumChallengeRunAsync(EditForumChallengeRunDTO r);

    public Task AddForumChallengeRunAsync(EditForumChallengeRunDTO r);

    public Task DeleteForumChallengeRunAsync(string runId);

    public IOrderedEnumerable<ForumChallengeLeaderboardDto> GetForumChallengeLeaderBoardDto(
        IEnumerable<ForumChallengeRunDTO> runs);

    public string GetForumChallengeLeaderboard(IOrderedEnumerable<ForumChallengeLeaderboardDto> lb);

    public Task<string> GetSeriesLeaderboardAsync(string seriesId);
}