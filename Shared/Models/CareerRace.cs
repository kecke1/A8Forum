using Microsoft.Azure.CosmosRepository;
using Shared.Enums;

namespace Shared.Models;

public class CareerRace : Item
{
    public CareerRaceTypeEnum CareerRaceType { get; set; } = CareerRaceTypeEnum.Classic;
    public string TrackId { get; set; }
    public ClassEnum? Class { get; set; }
    public string SeasonId { get; set; }
    public bool Limitations { get; set; } = false;
    public string LimitationsDescription { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
}