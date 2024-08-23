using Microsoft.Azure.CosmosRepository;
using Shared.Dto;
using Shared.Mappers;
using Shared.Models;

namespace Shared.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Member> _memberRepository;
    private readonly IRepository<Season> _seasonRepository;
    private readonly IRepository<Track> _trackRepository;
    private readonly IRepository<RankBracket> _rankBracketRepository;
    private readonly IRepository<CareerRace> _careerRaceRepository;

    public MasterDataService(IRepository<Vehicle> vehicleRepository,
        IRepository<Member> memberRepository,
        IRepository<Season> seasonRepository,
        IRepository<Track> trackRepository,
        IRepository<RankBracket> rankBracketRepository,
        IRepository<CareerRace> careerRaceRepository)
    {
        _vehicleRepository = vehicleRepository;
        _memberRepository = memberRepository;
        _seasonRepository = seasonRepository;
        _trackRepository = trackRepository;
        _rankBracketRepository = rankBracketRepository;
        _careerRaceRepository = careerRaceRepository;
    }

    public async Task AddVehicleAsync(VehicleDTO v)
    {
        await _vehicleRepository.CreateAsync(v.ToVehicleEntity());
    }

    public async Task DeleteVehicleAsync(string vehicleId)
    {
        await _vehicleRepository.DeleteAsync(vehicleId);
    }

    public async Task<VehicleDTO> GetVehicleAsync(string vehicleId)
    {
        var v = await _vehicleRepository.GetAsync(vehicleId);
        return v.ToDto();
    }

    public async Task<IEnumerable<VehicleDTO>> GetVehiclesAsync()
    {
        var v = await _vehicleRepository.GetAsync(x => true);
        return v.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateVehicleAsync(VehicleDTO v)
    {
        await _vehicleRepository.UpdateAsync(v.ToVehicleEntity());
    }

    public async Task AddMemberAsync(MemberDTO m)
    {
        await _memberRepository.CreateAsync(m.ToMemberEntity());
    }

    public async Task DeleteMemberAsync(string memberId)
    {
        // var m = await _memberRepository.GetAsync(memberId);

        await _memberRepository.DeleteAsync(memberId);
    }

    public async Task<MemberDTO> GetMemberAsync(string memberId)
    {
        var m = await _memberRepository.GetAsync(memberId);
        return m.ToDto();
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        var m = await _memberRepository.GetAsync(x => true);
        return m.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateMemberAsync(MemberDTO m)
    {
        await _memberRepository.UpdateAsync(m.ToMemberEntity());
    }

    public async Task AddSeasonAsync(SeasonDTO s)
    {
        await _seasonRepository.CreateAsync(s.ToSeasonEntity());
    }

    public async Task DeleteSeasonAsync(string seasonId)
    {
        await _seasonRepository.DeleteAsync(seasonId);
    }

    public async Task<SeasonDTO> GetSeasonAsync(string seasonId)
    {
        var s = await _seasonRepository.GetAsync(seasonId);
        return s.ToDto();
    }

    public async Task<IEnumerable<SeasonDTO>> GetSeasonsAsync()
    {
        var s = await _seasonRepository.GetAsync(x => true);
        return s.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateSeasonAsync(SeasonDTO s)
    {
        await _seasonRepository.UpdateAsync(s.ToSeasonEntity());
    }

    public async Task<IEnumerable<TrackDTO>> GetTracksAsync()
    {
        var t = await _trackRepository.GetAsync(x => true);
        return t.Select(x => x.ToDto()).ToArray();
    }

    public async Task<TrackDTO> GetTrackAsync(string trackId)
    {
        var t = await _trackRepository.GetAsync(trackId);
        return t.ToDto();
    }

    public async Task UpdateTrackAsync(TrackDTO t)
    {
        await _trackRepository.UpdateAsync(t.ToTrackEntity());
    }

    public async Task AddTrackAsync(TrackDTO t)
    {
        await _trackRepository.CreateAsync(t.ToTrackEntity());
    }

    public async Task DeleteTrackAsync(string trackId)
    {
        await _trackRepository.DeleteAsync(trackId);
    }

    public async Task<IEnumerable<RankBracketDTO>> GetRankBracketsAsync()
    {
        var r = await _rankBracketRepository.GetAsync(x => true);
        return r.Select(x => x.ToDto()).ToArray();
    }

    public async Task<RankBracketDTO> GetRankBracketAsync(string rankBracketId)
    {
        var r = await _rankBracketRepository.GetAsync(rankBracketId);
        return r.ToDto();
    }

    public async Task UpdateRankBracketAsync(RankBracketDTO r)
    {
        await _rankBracketRepository.UpdateAsync(r.ToRankBracketEntity());
    }

    public async Task AddRankBracketAsync(RankBracketDTO r)
    {
        await _rankBracketRepository.CreateAsync(r.ToRankBracketEntity());
    }

    public async Task DeleteRankBracketAsync(string rankBracketId)
    {
        await _rankBracketRepository.DeleteAsync(rankBracketId);
    }

    public async Task<IEnumerable<CareerRaceDTO>> GetCareerRacesAsync()
    {
        var r = await _careerRaceRepository.GetAsync(x => true);
        var tracks = await _trackRepository.GetAsync(x => true);
        var seasons = await _seasonRepository.GetAsync(x => true);

        return r
            .Select(x => x
            .ToDto(tracks
            .Single(y => y.Id == x.TrackId).ToDto(),
            seasons.Single(y => y.Id == x.SeasonId).ToDto()))
            .ToArray();
    }

    public async Task<CareerRaceDTO> GetCareerRaceAsync(string careerRaceId)
    {
        var r = await _careerRaceRepository.GetAsync(careerRaceId);
        var track = await _trackRepository.GetAsync(r.TrackId);
        var season = await _seasonRepository.GetAsync(r.SeasonId);
        return r.ToDto(track.ToDto(), season.ToDto());
    }

    public async Task UpdateCareerRaceAsync(CareerRaceDTO r)
    {
        await _careerRaceRepository.UpdateAsync(r.ToCareerRaceEntity());
    }

    public async Task AddCareerRaceAsync(CareerRaceDTO r)
    {
        await _careerRaceRepository.CreateAsync(r.ToCareerRaceEntity());
    }

    public async Task DeleteCareerRaceAsync(string careerRaceId)
    {
        await _careerRaceRepository.DeleteAsync(careerRaceId);
    }
}