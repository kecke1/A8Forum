using Shared.Enums;

namespace Shared.Dto;

public class CareerRaceDTO
{
    public required string Id { get; set; }
    public CareerRaceTypeEnum CareerRaceType { get; set; }
    public required TrackDTO Track { get; set; }
    public ClassEnum? Class { get; set; }
    public required SeasonDTO Season { get; set; }
    public bool Limitations { get; set; }
    public required string LimitationsDescription { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}