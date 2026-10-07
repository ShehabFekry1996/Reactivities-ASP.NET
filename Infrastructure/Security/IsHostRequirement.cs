using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }


    public class IsHostRequirementHandler(AppDBContext dbContext,IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<IsHostRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return;
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.GetRouteValue("id") is not string activityId)
                return;
            var attendee = await dbContext.ActivityAttendees.SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == activityId);
            if (attendee is null)
                return;
            if (attendee.IsHost)
                context.Succeed(requirement);
        }
    }
}
