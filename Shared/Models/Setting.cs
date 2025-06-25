 using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;
public class Setting : Item
{
    public required bool ShowExceptions { get; set; }
}
