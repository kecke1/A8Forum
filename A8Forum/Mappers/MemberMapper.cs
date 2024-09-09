using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class MemberMapper
{
    public static MemberDTO ToDto(this MemberViewModel member)
    {
        return new MemberDTO
        {
            Guest = member.Guest,
            MemberDisplayName = member.MemberDisplayName,
            MemberName = member.MemberName,
            Id = member.MemberId
        };
    }

    public static MemberViewModel ToMemberViewModel(this MemberDTO model)
    {
        var v = new MemberViewModel
        {
            Guest = model.Guest,
            MemberDisplayName = model.MemberDisplayName,
            MemberName = model.MemberName
        };

        if (model.Id != null)
            v.MemberId = model.Id;

        return v;
    }
}