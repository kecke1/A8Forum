using Microsoft.Azure.CosmosRepository;
using Shared.Dto;
using Shared.Models;

namespace Shared.Services;

public class DataManagementService : IDataManagementService
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Member> _memberRepository;
    private readonly IRepository<Season> _seasonRepository;
    private readonly IRepository<Track> _trackRepository;
    private readonly IRepository<RankBracket> _rankBracketRepository;
    private readonly IRepository<CareerRace> _careerRaceRepository;
    private readonly IRepository<ForumChallenge> _forumChallengeRepository;
    private readonly IRepository<ForumChallengeRun> _forumChallengeRunRepository;
    private readonly IRepository<Series> _seriesRepository;
    private readonly IRepository<GauntletRun> _gauntletRunRepository;
    private readonly IRepository<GiftLink> _giftLinkRepository;
    private readonly IRepository<GiftLinkProvider> _giftLinkProviderRepository;

    public DataManagementService(IRepository<ForumChallenge> forumChallengeRepository,
        IRepository<ForumChallengeRun> forumChallengeRunRepository,
        IRepository<Series> seriesRepository,
        IRepository<GauntletRun> gauntletRunRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Member> memberRepository,
        IRepository<Season> seasonRepository,
        IRepository<Track> trackRepository,
        IRepository<RankBracket> rankBracketRepository,
        IRepository<CareerRace> careerRaceRepository
        )
    {
        _forumChallengeRepository = forumChallengeRepository;
        _seasonRepository = seasonRepository;
        _trackRepository = trackRepository;
        _rankBracketRepository = rankBracketRepository;
        _forumChallengeRunRepository = forumChallengeRunRepository;
        _seriesRepository = seriesRepository;
        _gauntletRunRepository = gauntletRunRepository;
        _vehicleRepository = vehicleRepository;
        _memberRepository = memberRepository;
        _careerRaceRepository = careerRaceRepository;
    }

    public async Task<ExportDataDto> ExportData()
    {
        var export = new ExportDataDto
        {
            Seasons = await _seasonRepository.GetAsync(x => true),
            ForumChallenges = await _forumChallengeRepository.GetAsync(x => true),
            Tracks = await _trackRepository.GetAsync(x => true),
            RankBrackets = await _rankBracketRepository.GetAsync(x => true),
            ForumChallengeRuns = await _forumChallengeRunRepository.GetAsync(x => true),
            Series = await _seriesRepository.GetAsync(x => true),
            GautletRuns = await _gauntletRunRepository.GetAsync(x => true),
            Vehicles = await _vehicleRepository.GetAsync(x => true),
            Members = await _memberRepository.GetAsync(x => true),
            CareerRaces = await _careerRaceRepository.GetAsync(x => true)
        };

        return export;
    }
}