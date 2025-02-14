namespace Shared.Dto;

public class SprintImportDTO
{
    public required string ImportedText { get; set; }
    public required string Seperator { get; set; } = ";";
    public required string PostUrl { get; set; }
    public int? RunDateColumn { get; set; }
    public string RunDateFormat { get; set; }
    public int TrackColumn { get; set; } = 1;
    public int TimeColumn { get; set; } = 2;
    public int VehicleColumn { get; set; } = 3;
    public required string MemberId { get; set; }
    public int? MediaLinkColumn { get; set; }
}