using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class EditActivity
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required Activity Activity { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper) : IRequestHandler<Command,Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.
                    FindAsync([request.Activity.Id], cancellationToken);
                if (activity == null)
                {
                    return Result<Unit>.Failure("Activity not found", 404);
                }
                mapper.Map(request.Activity, activity);
                var result =await context.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                    return Result<Unit>.Failure("Failed to delete activity", 400);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
