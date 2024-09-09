using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class VehicleMapper
{
    public static VehicleDTO ToDto(this VehicleViewModel vehicle)
    {
        return new VehicleDTO
        {
            Id = vehicle.VehicleId,
            Name = vehicle.Name,
            ShortName = vehicle.ShortName,
            Keyword = vehicle.Keyword,
            Url = vehicle.Url,
            MaxRank = vehicle.MaxRank
        };
    }

    public static VehicleViewModel ToVehicleViewModel(this VehicleDTO model)
    {
        var v = new VehicleViewModel
        {
            Name = model.Name,
            ShortName = model.ShortName,
            Keyword = model.Keyword,
            Url = model.Url,
            MaxRank = model.MaxRank,
            VehicleId = model.Id
        };

        if (model.Id != null)
            v.VehicleId = model.Id;

        return v;
    }
}