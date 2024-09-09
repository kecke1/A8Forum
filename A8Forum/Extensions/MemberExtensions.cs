using A8Forum.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A8Forum.Extensions;

public static class MemberExtensions
{
    public static SelectList ToSelectList(this IEnumerable<MemberViewModel> m, string? selectedValue)
    {
        return new SelectList(m.OrderBy(x => x.MemberDisplayName), "MemberId", "MemberDisplayName", selectedValue);
    }
}