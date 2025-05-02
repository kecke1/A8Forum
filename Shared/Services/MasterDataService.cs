using Microsoft.Azure.CosmosRepository;
using Shared.Dto;
using Shared.Mappers;
using Shared.Models;

namespace Shared.Services;

public class MasterDataService(IRepository<Vehicle> vehicleRepository,
        IRepository<Member> memberRepository,
        IRepository<Season> seasonRepository,
        IRepository<Track> trackRepository,
        IRepository<RankBracket> rankBracketRepository,
        IRepository<CareerRace> careerRaceRepository)
    : IMasterDataService
{
    public async Task AddVehicleAsync(VehicleDTO v)
    {
        await vehicleRepository.CreateAsync(v.ToVehicleEntity());
    }

    public async Task DeleteVehicleAsync(string vehicleId)
    {
        await vehicleRepository.DeleteAsync(vehicleId);
    }

    public async Task<VehicleDTO> GetVehicleAsync(string vehicleId)
    {
        var v = await vehicleRepository.GetAsync(vehicleId);
        return v.ToDto();
    }

    public async Task<IEnumerable<VehicleDTO>> GetVehiclesAsync()
    {
        var v = await vehicleRepository.GetAsync(x => true);
        return v.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateVehicleAsync(VehicleDTO v)
    {
        await vehicleRepository.UpdateAsync(v.ToVehicleEntity());
    }

    public async Task AddMemberAsync(MemberDTO m)
    {
        await memberRepository.CreateAsync(m.ToMemberEntity());
    }

    public async Task DeleteMemberAsync(string memberId)
    {
        var m = await GetMemberAsync(memberId);
        m.Deleted = true;
        await UpdateMemberAsync(m);
    }

    public async Task<MemberDTO> GetMemberAsync(string memberId)
    {
        var m = await memberRepository.GetAsync(memberId);
        return m.ToDto();
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        var m = await memberRepository.GetAsync(x => true);
        return m.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateMemberAsync(MemberDTO m)
    {
        await memberRepository.UpdateAsync(m.ToMemberEntity());
    }

    public async Task AddSeasonAsync(SeasonDTO s)
    {
        await seasonRepository.CreateAsync(s.ToSeasonEntity());
    }

    public async Task DeleteSeasonAsync(string seasonId)
    {
        await seasonRepository.DeleteAsync(seasonId);
    }

    public async Task<SeasonDTO> GetSeasonAsync(string seasonId)
    {
        var s = await seasonRepository.GetAsync(seasonId);
        return s.ToDto();
    }

    public async Task<IEnumerable<SeasonDTO>> GetSeasonsAsync()
    {
        var s = await seasonRepository.GetAsync(x => true);
        return s.Select(x => x.ToDto()).ToArray();
    }

    public async Task UpdateSeasonAsync(SeasonDTO s)
    {
        await seasonRepository.UpdateAsync(s.ToSeasonEntity());
    }

    public async Task<IEnumerable<TrackDTO>> GetTracksAsync(bool includeRegularTracks = true, bool includeSprintTracks = false)
    {
        var t = await trackRepository
            .GetAsync(x => true);

        t = t.Where(x => x.Sprint == includeSprintTracks || x.Sprint != includeRegularTracks);

        return t.Select(x => x.ToDto()).ToArray();
    }

    public async Task<TrackDTO> GetTrackAsync(string trackId)
    {
        var t = await trackRepository.GetAsync(trackId);
        return t.ToDto();
    }

    public async Task UpdateTrackAsync(TrackDTO t)
    {
        await trackRepository.UpdateAsync(t.ToTrackEntity());
    }

    public async Task AddTrackAsync(TrackDTO t)
    {
        await trackRepository.CreateAsync(t.ToTrackEntity());
    }

    public async Task DeleteTrackAsync(string trackId)
    {
        await trackRepository.DeleteAsync(trackId);
    }

    public async Task<IEnumerable<RankBracketDTO>> GetRankBracketsAsync()
    {
        var r = await rankBracketRepository.GetAsync(x => true);
        return r.Select(x => x.ToDto()).ToArray();
    }

    public async Task<RankBracketDTO> GetRankBracketAsync(string rankBracketId)
    {
        var r = await rankBracketRepository.GetAsync(rankBracketId);
        return r.ToDto();
    }

    public async Task UpdateRankBracketAsync(RankBracketDTO r)
    {
        await rankBracketRepository.UpdateAsync(r.ToRankBracketEntity());
    }

    public async Task AddRankBracketAsync(RankBracketDTO r)
    {
        await rankBracketRepository.CreateAsync(r.ToRankBracketEntity());
    }

    public async Task DeleteRankBracketAsync(string rankBracketId)
    {
        await rankBracketRepository.DeleteAsync(rankBracketId);
    }

    public async Task<IEnumerable<CareerRaceDTO>> GetCareerRacesAsync()
    {
        var r = await careerRaceRepository.GetAsync(x => true);
        var tracks = await trackRepository.GetAsync(x => true);
        var seasons = await seasonRepository.GetAsync(x => true);

        return r
            .Select(x => x
                .ToDto(tracks
                        .Single(y => y.Id == x.TrackId).ToDto(),
                    seasons.Single(y => y.Id == x.SeasonId).ToDto()))
            .ToArray();
    }

    public async Task<CareerRaceDTO> GetCareerRaceAsync(string careerRaceId)
    {
        var r = await careerRaceRepository.GetAsync(careerRaceId);
        var track = await trackRepository.GetAsync(r.TrackId);
        var season = await seasonRepository.GetAsync(r.SeasonId);
        return r.ToDto(track.ToDto(), season.ToDto());
    }

    public async Task UpdateCareerRaceAsync(CareerRaceDTO r)
    {
        await careerRaceRepository.UpdateAsync(r.ToCareerRaceEntity());
    }

    public async Task AddCareerRaceAsync(CareerRaceDTO r)
    {
        await careerRaceRepository.CreateAsync(r.ToCareerRaceEntity());
    }

    public async Task DeleteCareerRaceAsync(string careerRaceId)
    {
        await careerRaceRepository.DeleteAsync(careerRaceId);
    }
}