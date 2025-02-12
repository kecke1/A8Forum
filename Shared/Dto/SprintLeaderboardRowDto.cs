using System.Text.Json.Serialization;

namespace Shared.Dto;

public class SprintLeaderboardRowDto
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
    public required string PostUrl { get; set; }
    public required string MediaLink { get; set; }
    public DateTime? RunDate { get; set; }
    public DateTime PostDate { get; set; }
    public required string VehicleName { get; set; }
    public required string VehicleUrl { get; set; }

    [JsonIgnore]
    public required string TrackId { get; set; }

    public required string TrackName { get; set; }

    [JsonIgnore]
    public IOrderedEnumerable<SprintRunDTO> Runs { get; set; } = Enumerable.Empty<SprintRunDTO>().OrderBy(x => x);
}