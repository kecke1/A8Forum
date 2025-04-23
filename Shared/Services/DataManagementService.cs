using Microsoft.Azure.CosmosRepository;
using Shared.Dto;
using Shared.Models;

namespace Shared.Services;

public class DataManagementService(IRepository<ForumChallenge> forumChallengeRepository,
        IRepository<ForumChallengeRun> forumChallengeRunRepository,
        IRepository<Series> seriesRepository,
        IRepository<GauntletRun> gauntletRunRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Member> memberRepository,
        IRepository<Season> seasonRepository,
        IRepository<Track> trackRepository,
        IRepository<RankBracket> rankBracketRepository,
        IRepository<CareerRace> careerRaceRepository,
        IRepository<SprintRun> sprintRunRepository,
        IRepository<SprintTrackReferencePoint> sprintTrackReferencePointRepository)
    : IDataManagementService
{
    private readonly IRepository<GiftLinkProvider> _giftLinkProviderRepository;
    private readonly IRepository<GiftLink> _giftLinkRepository;

    public async Task<ExportDataDto> ExportData()
    {
        var export = new ExportDataDto
        {
            Seasons = await seasonRepository.GetAsync(x => true),
            ForumChallenges = await forumChallengeRepository.GetAsync(x => true),
            Tracks = await trackRepository.GetAsync(x => true),
            RankBrackets = await rankBracketRepository.GetAsync(x => true),
            ForumChallengeRuns = await forumChallengeRunRepository.GetAsync(x => true),
            Series = await seriesRepository.GetAsync(x => true),
            GautletRuns = await gauntletRunRepository.GetAsync(x => true),
            Vehicles = await vehicleRepository.GetAsync(x => true),
            Members = await memberRepository.GetAsync(x => true),
            CareerRaces = await careerRaceRepository.GetAsync(x => true),
            SprintRuns = await sprintRunRepository.GetAsync(x => true),
            SprintTrackReferencePoint = (await sprintTrackReferencePointRepository.GetAsync(x => true)).First()
        };

        return export;
    }

    public async Task ImportData(ExportDataDto data)
    {
        await seasonRepository.CreateAsync(data.Seasons);
        await rankBracketRepository.CreateAsync(data.RankBrackets);
        await vehicleRepository.CreateAsync(data.Vehicles);
        await memberRepository.CreateAsync(data.Members);
        await trackRepository.CreateAsync(data.Tracks);
        await careerRaceRepository.CreateAsync(data.CareerRaces);
        await forumChallengeRepository.CreateAsync(data.ForumChallenges);
        await forumChallengeRunRepository.CreateAsync(data.ForumChallengeRuns);
        await seriesRepository.CreateAsync(data.Series);
        await gauntletRunRepository.CreateAsync(data.GautletRuns);
        await sprintRunRepository.CreateAsync(data.SprintRuns);
        await sprintTrackReferencePointRepository.CreateAsync(data.SprintTrackReferencePoint);
    }
}