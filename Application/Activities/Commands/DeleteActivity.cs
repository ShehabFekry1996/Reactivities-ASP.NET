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
    public class DeleteActivity
    {
        public class Command : IRequest<Result<Unit>> // use Unit when you want to represent a void type 
        {
            public required string Id { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper) : IRequestHandler<Command,Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync([request.Id], cancellationToken);
                if (activity == null)
                    return Result<Unit>.Failure("Cannot find activity", 404);
                context.Remove(activity);
                var result = await context.SaveChangesAsync(cancellationToken) > 0; // returns task for save opration 
                // number of states written to db so more than 0 is success 
                if (!result)
                    return Result<Unit>.Failure("Failed to delete activity", 400);
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}
