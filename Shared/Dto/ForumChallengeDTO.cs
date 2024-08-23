namespace Shared.Dto
{
    public class ForumChallengeDTO
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Idate { get; set; }
        public string? Post { get; set; }
        public string? CustomTitle { get; set; }
        public bool Deleted { get; set; }
        public string MaxRank { get; set; }
        public TrackDTO Track { get; set; }
        public SeasonDTO Season { get; set; }
        public string Leaderboard { get; set; }
        public string TitleHtml => $"[p style='margin:0px;'][span style=\"white-space: nowrap;\"]{StartDate:dd.MM.yyyy}-{EndDate:dd.MM.yyyy}[/span][/p][p style='margin:0px;'][span style=\"white-space: nowrap;\"]{Track.TrackName} {Season.SeasonName}[/span][/p][p style='margin:0px;'][span style=\"white-space: nowrap;\"] {(!string.IsNullOrEmpty(CustomTitle) ? CustomTitle : $"max rank {MaxRank}")}[/span][/p]";

        public string Title => !string.IsNullOrEmpty(CustomTitle)
            ? CustomTitle :
              $"{StartDate:dd.MM.yyyy}-{EndDate:dd.MM.yyyy} {Track.TrackName} {Season.SeasonName} max rank {MaxRank}";
    }
}