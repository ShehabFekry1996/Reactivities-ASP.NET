using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class UserAccessor(IHttpContextAccessor httpContextAccessor,AppDBContext dbContext) : IUserAccessor
    {
        public async Task<User> GetUserAsync()
        {
            return await dbContext.Users.FindAsync(GetUserId())
                ?? throw new UnauthorizedAccessException("No user is logged in");
        }

        public string GetUserId()
        {
            return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) // gives you id
                ?? throw new Exception("No user found"); 
        }
    }
}
