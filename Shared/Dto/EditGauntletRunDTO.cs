namespace Shared.Dto;

public class EditGauntletRunDTO
{
    public string? Id { get; set; }
    public int Time { get; set; }
    public DateTime Idate { get; set; }
    public bool Deleted { get; set; }
    public required string TrackId { get; set; }
    public required string Vehicle1Id { get; set; }
    public required string Vehicle2Id { get; set; }
    public required string Vehicle3Id { get; set; }
    public string? Vehicle4Id { get; set; }
    public string? Vehicle5Id { get; set; }
    public required string MemberId { get; set; }
    public string? PostUrl { get; set; }
    public DateTime? RunDate { get; set; }
    public string? MediaLink { get; set; }
    public bool LapTimeVerified { get; set; }
    public bool A8Plus { get; set; }
}