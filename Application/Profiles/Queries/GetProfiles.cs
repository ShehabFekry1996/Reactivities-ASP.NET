using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries
{
    public class GetProfiles
    {
        public class Query : IRequest<Result<List<UserProfile>>>
        {
            public string? Search { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<List<UserProfile>>>
        {
            public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = context.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    query = query.Where(x => x.DisplayName.ToLower().Contains(search)
                        || (x.Bio != null && x.Bio.ToLower().Contains(search)));
                }

                var profiles = await query
                    .OrderByDescending(x => x.Followers.Count)
                    .ThenBy(x => x.DisplayName)
                    .Take(50)
                    .ProjectTo<UserProfile>(mapper.ConfigurationProvider,
                        new { currentUserId = userAccessor.GetUserId() })
                    .ToListAsync(cancellationToken);

                return Result<List<UserProfile>>.Success(profiles);
            }
        }
    }
}
