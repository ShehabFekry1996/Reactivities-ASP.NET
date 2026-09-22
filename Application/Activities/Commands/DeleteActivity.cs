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
        public class Command : IRequest
        {
            public required string Id { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.
                    FindAsync([request.Id], cancellationToken) ?? throw new Exception("Cannot find activity");
                context.Remove(activity);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
