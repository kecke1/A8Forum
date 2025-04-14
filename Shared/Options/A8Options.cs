namespace Shared.Options;

public class A8Options
{
    public required string VehiclesUrl { get; set; }
    public required string VehiclesBaseUrl { get; set; }
    public required string CosmosConnection { get; set; }
    public required string CosmosDb { get; set; }
    public required string AdminUser { get; set; }
    public required string AdminPassword { get; set; }
    public required string SetupCosmosDb { get; set; }
    public required string ImportFile { get; set; }
}