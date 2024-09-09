namespace Shared.Dto;

public class VehicleDTO
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required string Keyword { get; set; }
    public string? Url { get; set; }
    public int? MaxRank { get; set; }
}