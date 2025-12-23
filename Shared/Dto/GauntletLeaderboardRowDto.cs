using System.Text.Json.Serialization;

namespace Shared.Dto;

public class GauntletLeaderboardRowDto
{
    [JsonIgnore]
    public required string MemberId { get; set; }

    [JsonPropertyName("Name")]
    public required string MemberName { get; set; }

    [JsonPropertyName("DisplayName")]
    public required string MemberDisplayName { get; set; }

    public int Position { get; set; }
    public int PositionPoints { get; set; }
    public required string TimeString { get; set; }
    public int Time { get; set; }
    public bool LapTimeVerified { get; set; }
    public bool A8Plus { get; set; }
    public int? VipLevel { get; set; }
    public required string PostUrl { get; set; }
    public required string MediaLink { get; set; }
    public DateTime? RunDate { get; set; }
    public DateTime PostDate { get; set; }
    public required string VehicleName1 { get; set; }
    public required string VehicleUrl1 { get; set; }
    public required string VehicleName2 { get; set; }
    public required string VehicleUrl2 { get; set; }
    public required string VehicleName3 { get; set; }
    public required string VehicleUrl3 { get; set; }
    public required string VehicleName4 { get; set; }
    public required string VehicleUrl4 { get; set; }
    public required string VehicleName5 { get; set; }
    public required string VehicleUrl5 { get; set; }

    public bool Glitch { get; set; }
    public bool Shortcut { get; set; }

    [JsonIgnore]
    public required string TrackId { get; set; }

    public required string TrackName { get; set; }

    [JsonIgnore]
    public IOrderedEnumerable<GauntletRunDTO> Runs { get; set; } = Enumerable.Empty<GauntletRunDTO>().OrderBy(x => x);
}