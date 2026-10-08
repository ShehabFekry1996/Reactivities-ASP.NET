using Application.Activities.DTOS;
using Application.Interfaces;
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

        public class Handler(AppDBContext context,IMapper mapper,IUserAccessor userAccessor) : IRequestHandler<Query, List<ActivityDto>>
        {
            public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.Activities.ProjectTo<ActivityDto>(mapper.ConfigurationProvider
                    ,new {currentUserId = userAccessor.GetUserId()})
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
