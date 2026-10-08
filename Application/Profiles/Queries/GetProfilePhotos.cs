using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Queries
{
    public class GetProfilePhotos
    {
        public class Query : IRequest<Result<List<Photo>>>
        {
            public required string UserId { get; set; }
        }

        public class Handler(AppDBContext context, IUserAccessor userAccessor) : IRequestHandler<Query, Result<List<Photo>>>
        {
            public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var photos = await context.Photos
                    .Where(p => p.UserId == request.UserId)
                    .ToListAsync(cancellationToken);

                return Result<List<Photo>>.Success(photos);
            }

        }

    }
}