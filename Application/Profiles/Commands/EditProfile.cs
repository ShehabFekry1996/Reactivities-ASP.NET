using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Commands
{
    public class EditProfile
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string DisplayName { get; set; } = string.Empty;

            public string? Bio { get; set; }
        }

        public class Handler(AppDBContext context, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserAsync();

                user.DisplayName = request.DisplayName;
                user.Bio = request.Bio;

                context.Entry(user).State = EntityState.Modified;

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result ?
                    Result<Unit>.Success(Unit.Value) :
                    Result<Unit>.Failure("Problem updating profile", 400);
            }
        }
    }
}
