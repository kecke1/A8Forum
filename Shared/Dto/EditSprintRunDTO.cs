namespace Shared.Dto;

public class EditSprintRunDTO
{
    public string? Id { get; set; }
    public int Time { get; set; }
    public DateTime Idate { get; set; }
    public bool Deleted { get; set; }
    public required string TrackId { get; set; }
    public required string VehicleId { get; set; }
    public required string MemberId { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
    public int? VipLevel { get; set; }
    public bool Shortcut { get; set; }
    public bool Glitch { get; set; }
}