using Microsoft.AspNetCore.Mvc.Rendering;

namespace A8Forum.Extensions;

public static class SeasonExtensions
{
    public static SelectList ToSelectList(this IEnumerable<ViewModels.SeasonViewModel> s, string? selectedValue)
    {
        return new SelectList(s.OrderBy(x => x.SeasonName), "SeasonId", "SeasonName", selectedValue);
    }
}