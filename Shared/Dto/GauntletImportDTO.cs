namespace Shared.Dto;

public class GauntletImportDTO
{
    public string ImportedText { get; set; }
    public string Seperator { get; set; }
    public string VehiclesSeperator { get; set; }
    public string PostUrl { get; set; }
    public int RunDateColumn { get; set; }
    public string RunDateFormat { get; set; }
    public int TrackColumn { get; set; }
    public int TimeColumn { get; set; }
    public int VehiclesColumn { get; set; }
    public string MemberId { get; set; }
    public int VerifiedColumn { get; set; }
    public int MediaLinkColumn { get; set; }
}