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
            Id = member.MemberId,
            VipLevel = member.VipLevel,
            Deleted = member.Deleted,
            Hidden = member.Hidden,
            RacingNames = member.RacingNames?.Split(',').Select(x => x.Trim()) ?? [],
            FormerRacingNames = member.FormerRacingNames?.Split(',').Select(x => x.Trim()) ?? []
        };
    }

    public static MemberViewModel ToMemberViewModel(this MemberDTO model)
    {
        var v = new MemberViewModel
        {
            Guest = model.Guest,
            MemberDisplayName = model.MemberDisplayName,
            MemberName = model.MemberName,
            VipLevel = model.VipLevel,
            Deleted = model.Deleted,
            Hidden = model.Hidden,
            RacingNames = string.Join(", ", model.RacingNames),
            FormerRacingNames = string.Join(", ", model.FormerRacingNames)
        };

        if (model.Id != null)
            v.MemberId = model.Id;

        return v;
    }
}