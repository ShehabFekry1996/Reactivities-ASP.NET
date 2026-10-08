using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Queries
{
    public class GetProfile
    {
        public class Query : IRequest<Result<UserProfile>>
        {
            public required string UserId { get; set; }
        }

        public class Handle(AppDBContext context, IMapper mapper,IUserAccessor userAccessor) : IRequestHandler<Query, Result<UserProfile>>
        {
            async Task<Result<UserProfile>> IRequestHandler<Query, Result<UserProfile>>.Handle(Query request, CancellationToken cancellationToken)
            {
                var profile = await context.Users.ProjectTo<UserProfile>(mapper.ConfigurationProvider,new {currentUserId = userAccessor.GetUserId()})
                    .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                return profile == null ?
                    Result<UserProfile>.Failure("Profile nopt found", 404)
                    : Result<UserProfile>.Success(profile);
            }
        }
    }
}
