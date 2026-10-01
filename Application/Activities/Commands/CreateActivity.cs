using Application.Activities.DTOS;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            public required CreateActivityDto ActivityDto { get; set; }
        }

        public class Handler(AppDBContext context,IMapper mapper) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = mapper.Map<Activity>(request.ActivityDto);
                context.Activities.Add(activity);
                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                    return Result<string>.Failure("Failed to create activity", 400);
                return Result<string>.Success(activity.Id);
            }
        }
    }
}
