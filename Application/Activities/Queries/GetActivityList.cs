using Application.Activities.DTOS;
using Application.Activities.Queries;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Queries
{
    public class GetActivityList
    {
        public class Query : IRequest<Result<PagedList<ActivityDto, string>>>
        {
            public required ActivityParams Params { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper, IUserAccessor userAccessor)
            : IRequestHandler<Query, Result<PagedList<ActivityDto, string>>>
        {
            public async Task<Result<PagedList<ActivityDto, string>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = userAccessor.GetUserId();
                var ascending = request.Params.SortOrder == "asc";

                var query = context.Activities
                    .Where(x => x.Date >= request.Params.StartDate)
                    .AsQueryable();

                if (TryParseCursor(request.Params.Cursor, out var cursorDate, out var cursorId))
                {
                    query = ascending
                        ? query.Where(x => x.Date > cursorDate || (x.Date == cursorDate && string.Compare(x.Id, cursorId) >= 0))
                        : query.Where(x => x.Date < cursorDate || (x.Date == cursorDate && string.Compare(x.Id, cursorId) <= 0));
                }

                if (!string.IsNullOrEmpty(request.Params.Category))
                {
                    query = query.Where(x => x.Category == request.Params.Category);
                }

                if (!string.IsNullOrWhiteSpace(request.Params.Search))
                {
                    var search = request.Params.Search.Trim().ToLower();
                    query = query.Where(x => x.Title.ToLower().Contains(search)
                        || x.City.ToLower().Contains(search)
                        || x.Venue.ToLower().Contains(search));
                }

                query = request.Params.Filter switch
                {
                    "isGoing" => query.Where(x => x.Attendees.Any(a => a.UserId == userId)),
                    "isHost" => query.Where(x => x.Attendees.Any(a => a.IsHost && a.UserId == userId)),
                    _ => query
                };

                query = ascending
                    ? query.OrderBy(x => x.Date).ThenBy(x => x.Id)
                    : query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id);

                var activities = await query
                    .Take(request.Params.PageSize + 1)
                    .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userId })
                    .ToListAsync(cancellationToken);

                string? nextCursor = null;
                if (activities.Count > request.Params.PageSize)
                {
                    var last = activities.Last();
                    nextCursor = $"{last.Date.ToUniversalTime():O}|{last.Id}";
                    activities.RemoveAt(activities.Count - 1);
                }

                return Result<PagedList<ActivityDto, string>>.Success(new PagedList<ActivityDto, string>
                {
                    Items = activities,
                    NextCursor = nextCursor
                });
            }

            private static bool TryParseCursor(string? cursor, out DateTime date, out string id)
            {
                date = default;
                id = string.Empty;
                var parts = cursor?.Split('|');
                if (parts is not { Length: 2 }) return false;
                id = parts[1];
                return DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date);
            }
        }
    }
}
