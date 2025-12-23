
// TagHelpers/TrackSelectTagHelper.cs
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace A8Forum.TagHelpers;

[HtmlTargetElement("track-select", Attributes = ForAttributeName)]
public class TrackSelectTagHelper : TagHelper
{
    private const string ForAttributeName = "asp-for";

    [HtmlAttributeName(ForAttributeName)] public ModelExpression For { get; set; } = default!;

    [HtmlAttributeName("multiple")] public bool IsMultiple { get; set; }

    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; } = "Select track";

    [HtmlAttributeName("allow-clear")] public bool AllowClear { get; set; } = true;

    [HtmlAttributeName("disabled")] public bool Disabled { get; set; }

    [HtmlAttributeName("required")] public bool Required { get; set; }

    [HtmlAttributeName("width")] public string Width { get; set; } = "100%";

    // Optional: override endpoint(s) if needed
    [HtmlAttributeName("ajax-url")] public string AjaxUrl { get; set; } = "/lookup/tracks";

    [HtmlAttributeName("resolve-url")] public string ResolveUrl { get; set; } = "/lookup/tracks/byids";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "select";
        output.TagMode = TagMode.StartTagAndEndTag;

        // name/id from asp-for (binds to int? or List<int>)
        var name = For.Name;
        var id = For.Name.Replace(".", "_");

        output.Attributes.SetAttribute("name", name);
        output.Attributes.SetAttribute("id", id);

        if (IsMultiple)
            output.Attributes.SetAttribute("multiple", "multiple");

        if (Disabled)
            output.Attributes.SetAttribute("disabled", "disabled");

        if (Required)
            output.Attributes.SetAttribute("required", "required");

        // Hook for our initializer
        output.Attributes.SetAttribute("data-select2", "track");
        output.Attributes.SetAttribute("data-ajax-url", AjaxUrl);
        output.Attributes.SetAttribute("data-resolve-url", ResolveUrl);
        output.Attributes.SetAttribute("data-placeholder", Placeholder ?? "Select track");
        output.Attributes.SetAttribute("data-allow-clear", AllowClear ? "true" : "false");
        output.Attributes.SetAttribute("data-width", Width);

        // If Model has a value, render a selected <option> so non-JS fallback and SSR keep it visible.
        var modelValue = For.Model;
        if (modelValue is IEnumerable<int> list)
        {
            foreach (var v in list)
            {
                output.Content.AppendHtml($"<option value=\"{v}\" selected></option>");
            }
        }
        else if (modelValue is int vInt && vInt > 0)
        {
            output.Content.AppendHtml($"<option value=\"{vInt}\" selected></option>");
        }
       // else if (modelValue is int ? vN && vN.HasValue && vN.Value > 0)
       // {
       //     output.Content.AppendHtml($"<option value=\"{vN.Value}\" selected></option>");
       // }
        // If no value: leave empty; Select2 will show placeholder
    }
}