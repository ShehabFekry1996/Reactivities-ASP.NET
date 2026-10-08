using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Commands
{
    public class AddPhoto
    {
        public class Command : IRequest<Result<Photo>>
        {
            public required IFormFile File { get; set; }
        }

        public class Handler(IUserAccessor userAccessor, IPhotoService photoService,AppDBContext context) : IRequestHandler<Command, Result<Photo>>
        {

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserAsync();
                var photoUploadResult = await photoService.UploadPhotoAsync(request.File);

                if (photoUploadResult == null)
                    return Result<Photo>.Failure("Failed to upload photo", 400);
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublidId,
                    UserId = user.Id
                };
                user.ImageUrl ??= photo.Url;
                context.Photos.Add(photo);
                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                return result ? 
                    Result<Photo>.Success(photo):
                    Result<Photo>.Failure("Failed to add photo", 400)
                    ;
            }

        }
    }
}
