using A8Forum.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A8Forum.Extensions;

public static class TrackExtensions
{
    public static SelectList ToSelectList(this IEnumerable<TrackViewModel> t, string? selectedValue)
    {
        return new SelectList(t.OrderBy(x => x.TrackName), "TrackId", "TrackName", selectedValue);
    }
}