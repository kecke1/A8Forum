using Microsoft.AspNetCore.Mvc.Rendering;

namespace A8Forum.Extensions;

public static class TrackExtensions
{
    public static SelectList ToSelectList(this IEnumerable<ViewModels.TrackViewModel> t, string? selectedValue)
    {
        return new SelectList(t.OrderBy(x => x.TrackName), "TrackId", "TrackName", selectedValue);
    }
}