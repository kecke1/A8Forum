namespace Shared.Dto;

public class TrackDTO
{
    public required string Id { get; set; }
    public required string TrackName { get; set; }
    public required bool Sprint { get; set; } = false;
    public int? Order { get; set; }
}