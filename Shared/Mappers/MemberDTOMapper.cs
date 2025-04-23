using Shared.Dto;
using Shared.Models;

namespace Shared.Mappers;

internal static class MemberDTOMapper
{
    public static MemberDTO ToDto(this Member member)
    {
        return new MemberDTO
        {
            Id = member.Id,
            MemberName = member.MemberName,
            MemberDisplayName = member.MemberDisplayName,
            Guest = member.Guest,
            RacingNames = member.RacingNames,
            VipLevel = member.VipLevel
        };
    }

    public static Member ToMemberEntity(this MemberDTO model)
    {
        var m = new Member
        {
            MemberName = model.MemberName,
            MemberDisplayName = model.MemberDisplayName,
            Guest = model.Guest,
            VipLevel = model.VipLevel,
            RacingNames = model.RacingNames,
        };
        if (!string.IsNullOrEmpty(model.Id))
            m.Id = model.Id;

        return m;
    }
}