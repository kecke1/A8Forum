namespace Shared.Dto;

public class SprintRunDTO
{
    public string? Id { get; set; }
    public int Time { get; set; }
    public DateTime Idate { get; set; }
    public bool Deleted { get; set; }
    public TrackDTO? Track { get; set; }
    public VehicleDTO? Vehicle { get; set; }
    public MemberDTO? Member { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
    public int? VipLevel { get; set; }
}