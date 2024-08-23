using Shared.Dto;

namespace Shared.Mappers
{
    internal static class ForumChallengeRunDTOMapper
    {
        public static ForumChallengeRunDTO ToDto(this Shared.Models.ForumChallengeRun run,
            ForumChallengeDTO c,
            MemberDTO m,
            VehicleDTO v)
        {
            return new ForumChallengeRunDTO
            {
                Id = run.Id,
                Time = run.Time,
                Idate = run.Idate,
                Post = run.Post,
                Deleted = run.Deleted,
                ForumChallenge = c,
                Member = m,
                Vehicle = v
            };
        }

        public static Shared.Models.ForumChallengeRun ToForumChallengeRunEntity(this ForumChallengeRunDTO model)
        {
            var r = new Shared.Models.ForumChallengeRun
            {
                Time = model.Time,
                Idate = model.Idate,
                Post = model.Post,
                Deleted = model.Deleted,
                ForumChallengeId = model.ForumChallenge.Id ?? throw new NullReferenceException(),
                MemberId = model.Member.Id ?? throw new NullReferenceException(),
                VehicleId = model.Vehicle.Id ?? throw new NullReferenceException()
            };

            if (!string.IsNullOrEmpty(model.Id))
            {
                r.Id = model.Id;
            }

            return r;
        }
    }
}