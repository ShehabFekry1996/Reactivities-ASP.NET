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
                    .Where(p => p.UserId == request.UserId).ToListAsync(cancellationToken);
                if (photos == null || photos.Count == 0)
                {
                    return Result<List<Photo>>.Failure("No photos found for the specified user.", 404);
                }
                // Assuming you want to return all photos or you can modify this logic as needed
                return Result<List<Photo>>.Success(photos);
            }

        }

    }
}