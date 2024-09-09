using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class GiftLinkMapper
{
    public static GiftLinkDTO ToDto(this GiftLinkViewModel g)
    {
        return new GiftLinkDTO
        {
            Id = g.GiftLinkId,
            Deleted = g.Deleted,
            Url = g.Url,
            GiftLinkProvider = g.GiftLinkProvider.ToDto(),
            Idate = g.Idate,
            Member = g.SubmitedBy.ToDto(),
            Month = g.Month,
            Notes = g.Notes
        };
    }

    public static GiftLinkViewModel ToGiftLinkViewModel(this GiftLinkDTO model)
    {
        return new GiftLinkViewModel
        {
            GiftLinkId = model.Id,
            Deleted = model.Deleted,
            Url = model.Url,
            GiftLinkProvider = model.GiftLinkProvider.ToGiftLinkProviderViewModel(),
            Idate = model.Idate,
            Notes = model.Notes,
            Month = model.Month,
            SubmitedBy = model.Member.ToMemberViewModel(),
            IgnoreDuplicateValidation = false
        };
    }
}