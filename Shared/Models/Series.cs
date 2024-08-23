using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class Series : Item
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}