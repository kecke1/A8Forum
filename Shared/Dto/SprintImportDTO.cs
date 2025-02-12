namespace Shared.Dto;

public class SprintImportDTO
{
    public required string ImportedText { get; set; }
    public required string Seperator { get; set; }
    public required string PostUrl { get; set; }
    public int RunDateColumn { get; set; }
    public required string RunDateFormat { get; set; }
    public int TrackColumn { get; set; }
    public int TimeColumn { get; set; }
    public int VehicleColumn { get; set; }
    public required string MemberId { get; set; }
    public int MediaLinkColumn { get; set; }
}