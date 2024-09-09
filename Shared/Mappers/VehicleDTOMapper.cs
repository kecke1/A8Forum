using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class VehicleDTOMapper
{
    public static VehicleDTO ToDto(this Vehicle vehicle)
    {
        return new VehicleDTO
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            ShortName = vehicle.ShortName,
            Keyword = vehicle.Keyword,
            Url = vehicle.Url,
            MaxRank = vehicle.MaxRank
        };
    }

    public static Vehicle ToVehicleEntity(this VehicleDTO model)
    {
        var v = new Vehicle
        {
            Name = model.Name,
            ShortName = model.ShortName,
            Keyword = model.Keyword,
            Url = model.Url,
            MaxRank = model.MaxRank
        };

        if (model.Id != null)
            v.Id = model.Id;

        return v;
    }
}