using Application.Activities.DTOS;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class AddComment
    {

        public class Command : IRequest<Result<CommentDto>>
        {
            public required string Body { get; set; }

            public required string ActivityId { get; set; }
        }

        public class Handler(IMapper mapper, AppDBContext context,IUserAccessor userAccessor) : IRequestHandler<Command, Result<CommentDto>>
        {
            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities
                    .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityId,cancellationToken);

                if(activity is null)
                    return Result<CommentDto>.Failure("Couldn't find activity", 404);

                var user = await userAccessor
                    .GetUserAsync();
                var comment = new Comment 
                    { 
                        Body = request.Body,
                        ActivityId = activity.Id,
                        UserId = user.Id
                    };
                activity.Comments.Add(comment);
                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result?
                         Result<CommentDto>.Success(mapper.Map<CommentDto>(comment)) :
                         Result<CommentDto>.Failure("Failed to add comment", 500);
            }
        }
    }
}
