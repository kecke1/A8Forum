namespace Shared.Dto
{
    public class ForumChallengeRunDTO
    {
        public string Id { get; set; }
        public int Time { get; set; }
        public DateTime Idate { get; set; }
        public string? Post { get; set; }
        public bool Deleted { get; set; }
        public ForumChallengeDTO ForumChallenge { get; set; }
        public MemberDTO Member { get; set; }
        public VehicleDTO? Vehicle { get; set; }
    }
}