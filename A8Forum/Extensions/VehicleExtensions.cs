using A8Forum.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A8Forum.Extensions;

public static class VehicleExtensions
{
    public static SelectList ToSelectList(this IEnumerable<VehicleViewModel> v, string? selectedValue)
    {
        return new SelectList(v.OrderBy(x => x.MaxRank), "VehicleId", "DisplayName", selectedValue);
    }
}