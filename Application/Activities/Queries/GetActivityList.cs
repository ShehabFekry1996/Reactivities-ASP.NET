using Application.Activities.DTOS;
using Application.Activities.Queries;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Queries
{
    public class GetActivityList
    {
        public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
        {
            public required ActivityParams Params { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper, IUserAccessor userAccessor)
            : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
        {
            public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = userAccessor.GetUserId();
                var ascending = request.Params.SortOrder == "asc";

                var query = context.Activities
                    .Where(x => x.Date >= request.Params.StartDate)
                    .AsQueryable();

                if (request.Params.Cursor.HasValue)
                {
                    query = ascending
                        ? query.Where(x => x.Date >= request.Params.Cursor.Value)
                        : query.Where(x => x.Date <= request.Params.Cursor.Value);
                }

                query = request.Params.Filter switch
                {
                    "isGoing" => query.Where(x => x.Attendees.Any(a => a.UserId == userId)),
                    "isHost" => query.Where(x => x.Attendees.Any(a => a.IsHost && a.UserId == userId)),
                    _ => query
                };

                query = ascending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);

                var activities = await query
                    .Take(request.Params.PageSize + 1)
                    .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userId })
                    .ToListAsync(cancellationToken);

                DateTime? nextCursor = null;
                if (activities.Count > request.Params.PageSize)
                {
                    nextCursor = activities.Last().Date;
                    activities.RemoveAt(activities.Count - 1);
                }

                return Result<PagedList<ActivityDto, DateTime?>>.Success(new PagedList<ActivityDto, DateTime?>
                {
                    Items = activities,
                    NextCursor = nextCursor
                });
            }
        }
    }
}
