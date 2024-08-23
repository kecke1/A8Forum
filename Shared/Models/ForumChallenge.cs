using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class ForumChallenge : Item
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime Idate { get; set; } = DateTime.Now;
    public string? Post { get; set; } = "";
    public string? CustomTitle { get; set; } = "";
    public bool Deleted { get; set; }
    public string MaxRank { get; set; } = "";
    public string TrackId { get; set; }
    public string SeasonId { get; set; }
}