using Shared.Dto;

namespace Shared.Services;

public interface IMasterDataService
{
    public Task<IEnumerable<VehicleDTO>> GetVehiclesAsync();

    public Task<VehicleDTO> GetVehicleAsync(string vehicleId);

    public Task UpdateVehicleAsync(VehicleDTO v);

    public Task AddVehicleAsync(VehicleDTO v);

    public Task DeleteVehicleAsync(string vehicleId);

    public Task<IEnumerable<MemberDTO>> GetMembersAsync();

    public Task<MemberDTO> GetMemberAsync(string memberId);

    public Task UpdateMemberAsync(MemberDTO m);

    public Task AddMemberAsync(MemberDTO m);

    public Task DeleteMemberAsync(string memberId);

    public Task<IEnumerable<SeasonDTO>> GetSeasonsAsync();

    public Task<SeasonDTO> GetSeasonAsync(string seasonId);

    public Task UpdateSeasonAsync(SeasonDTO s);

    public Task AddSeasonAsync(SeasonDTO s);

    public Task DeleteSeasonAsync(string seasonId);

    public Task<IEnumerable<TrackDTO>> GetTracksAsync();

    public Task<TrackDTO> GetTrackAsync(string trackId);

    public Task UpdateTrackAsync(TrackDTO t);

    public Task AddTrackAsync(TrackDTO t);

    public Task DeleteTrackAsync(string trackId);

    public Task<IEnumerable<RankBracketDTO>> GetRankBracketsAsync();

    public Task<RankBracketDTO> GetRankBracketAsync(string rankBracketId);

    public Task UpdateRankBracketAsync(RankBracketDTO r);

    public Task AddRankBracketAsync(RankBracketDTO r);

    public Task DeleteRankBracketAsync(string rankBracketId);

    public Task<IEnumerable<CareerRaceDTO>> GetCareerRacesAsync();

    public Task<CareerRaceDTO> GetCareerRaceAsync(string careerRaceId);

    public Task UpdateCareerRaceAsync(CareerRaceDTO r);

    public Task AddCareerRaceAsync(CareerRaceDTO r);

    public Task DeleteCareerRaceAsync(string careerRaceId);
}