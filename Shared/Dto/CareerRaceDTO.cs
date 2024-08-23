using Shared.Enums;

namespace Shared.Dto
{
    public class CareerRaceDTO
    {
        public string Id { get; set; }
        public CareerRaceTypeEnum CareerRaceType { get; set; }
        public TrackDTO Track { get; set; }
        public ClassEnum? Class { get; set; }
        public SeasonDTO Season { get; set; }
        public bool Limitations { get; set; }
        public string LimitationsDescription { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}