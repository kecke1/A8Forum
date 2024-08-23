namespace Shared.Dto
{
    public class GauntletRunDTO
    {
        public string Id { get; set; }
        public int Time { get; set; }
        public DateTime Idate { get; set; }
        public bool Deleted { get; set; }
        public TrackDTO Track { get; set; }
        public VehicleDTO Vehicle1 { get; set; }
        public VehicleDTO Vehicle2 { get; set; }
        public VehicleDTO Vehicle3 { get; set; }
        public VehicleDTO? Vehicle4 { get; set; }
        public VehicleDTO? Vehicle5 { get; set; }
        public MemberDTO Member { get; set; }
        public string? PostUrl { get; set; }
        public DateTime? RunDate { get; set; }
        public string? MediaLink { get; set; }
        public bool LapTimeVerified { get; set; }
    }
}