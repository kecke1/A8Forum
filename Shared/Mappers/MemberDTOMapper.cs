using Shared.Dto;

namespace Shared.Mappers
{
    internal static class MemberDTOMapper
    {
        public static MemberDTO ToDto(this Shared.Models.Member member)
        {
            return new MemberDTO
            {
                Id = member.Id,
                MemberName = member.MemberName,
                MemberDisplayName = member.MemberDisplayName,
                Guest = member.Guest
            };
        }

        public static Shared.Models.Member ToMemberEntity(this MemberDTO model)
        {
            var m = new Shared.Models.Member
            {
                MemberName = model.MemberName,
                MemberDisplayName = model.MemberDisplayName,
                Guest = model.Guest
            };
            if (!string.IsNullOrEmpty(model.Id))
            {
                m.Id = model.Id;
            }

            return m;
        }
    }
}