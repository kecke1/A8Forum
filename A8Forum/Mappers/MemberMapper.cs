using Shared.Dto;

namespace A8Forum.Mappers;

internal static class MemberMapper
{
    public static MemberDTO ToDto(this ViewModels.MemberViewModel member)
    {
        return new MemberDTO
        {
            Guest = member.Guest,
            MemberDisplayName = member.MemberDisplayName,
            MemberName = member.MemberName,
            Id = member.MemberId
        };
    }

    public static ViewModels.MemberViewModel ToMemberViewModel(this MemberDTO model)
    {
        var v = new ViewModels.MemberViewModel
        {
            Guest = model.Guest,
            MemberDisplayName = model.MemberDisplayName,
            MemberName = model.MemberName
        };

        if (model.Id != null)
        {
            v.MemberId = model.Id;
        }

        return v;
    }
}