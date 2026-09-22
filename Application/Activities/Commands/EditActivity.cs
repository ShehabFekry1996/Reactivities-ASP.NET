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
        public class Command : IRequest
        {
            public required Activity Activity { get; set; }
        }

        public class Handler(AppDBContext context, IMapper mapper) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.
                    FindAsync(
                    [request.Activity.Id], cancellationToken) ?? throw new Exception("Cannot find activity");
                mapper.Map(request.Activity, activity);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
