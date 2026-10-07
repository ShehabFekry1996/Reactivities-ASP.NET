using Application.Activities.DTOS;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public class Query : IRequest<List<ActivityDto>>
        {

        }

        public class Handler(AppDBContext context,IMapper mapper) : IRequestHandler<Query, List<ActivityDto>>
        {
            public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.Activities.ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
