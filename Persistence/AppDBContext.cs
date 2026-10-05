using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<User>(options)
    {
        public required DbSet<Activity> Activities { get; set;  }
    }
}
