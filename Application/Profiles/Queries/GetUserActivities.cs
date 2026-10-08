using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries
{
    public class GetUserActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public required string UserId { get; set; }

            public string Filter { get; set; } = "future";
        }

        public class Handler(AppDBContext context, IMapper mapper) : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = context.ActivityAttendees
                    .Where(x => x.UserId == request.UserId)
                    .Select(x => x.Activity);

                var today = DateTime.UtcNow;

                query = request.Filter switch
                {
                    "past" => query.Where(x => x.Date <= today).OrderByDescending(x => x.Date),
                    "hosting" => context.ActivityAttendees
                        .Where(x => x.UserId == request.UserId && x.IsHost)
                        .Select(x => x.Activity)
                        .OrderBy(x => x.Date),
                    _ => query.Where(x => x.Date >= today).OrderBy(x => x.Date)
                };

                var activities = await query
                    .ProjectTo<UserActivityDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<UserActivityDto>>.Success(activities);
            }
        }
    }
}
