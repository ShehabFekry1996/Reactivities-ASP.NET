using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Commands
{
    public class SetMainPhoto
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string PhotoId { get; set; }
        }

        public class Handler(AppDBContext context, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserWithPhotosAsnyc();

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.PhotoId);

                if (photo == null)
                    return Result<Unit>.Failure("Cannot find photo", 404);
                user.ImageUrl = photo.Url;

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result ? 
                    Result<Unit>.Success(Unit.Value) : 
                    Result<Unit>.Failure("Problem setting main photo", 400);

            }
        }
    }
}
