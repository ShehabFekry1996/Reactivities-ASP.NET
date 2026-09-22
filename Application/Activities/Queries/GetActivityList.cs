using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries
{
    public class GetActivityList
    {
        public class Query : IRequest<List<Activity>>
        {

        }

        public class Handler(AppDBContext context) : IRequestHandler<Query, List<Activity>>
        {
            public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.Activities.ToListAsync(cancellationToken);
            }
        }
    }
}
