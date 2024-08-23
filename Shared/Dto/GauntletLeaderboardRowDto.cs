using System.Text.Json.Serialization;

namespace Shared.Dto;

public class GauntletLeaderboardRowDto
{
    [JsonIgnore]
    public string MemberId { get; set; }

    [JsonPropertyName("Name")]
    public string MemberName { get; set; }

    [JsonPropertyName("DisplayName")]
    public string MemberDisplayName { get; set; }

    public int Position { get; set; }
    public int PositionPoints { get; set; }
    public string TimeString { get; set; }
    public int Time { get; set; }
    public bool LapTimeVerified { get; set; }
    public string PostUrl { get; set; }
    public string MediaLink { get; set; }
    public DateTime? RunDate { get; set; }
    public DateTime PostDate { get; set; }
    public string VehicleName1 { get; set; }
    public string VehicleUrl1 { get; set; }
    public string VehicleName2 { get; set; }
    public string VehicleUrl2 { get; set; }
    public string VehicleName3 { get; set; }
    public string VehicleUrl3 { get; set; }
    public string VehicleName4 { get; set; }
    public string VehicleUrl4 { get; set; }
    public string VehicleName5 { get; set; }
    public string VehicleUrl5 { get; set; }

    [JsonIgnore]
    public string TrackId { get; set; }

    public string TrackName { get; set; }

    [JsonIgnore]
    public IOrderedEnumerable<GauntletRunDTO> Runs { get; set; } = Enumerable.Empty<GauntletRunDTO>().OrderBy(x => x);
}