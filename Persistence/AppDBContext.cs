using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public required DbSet<Activity> Activities { get; set;  }
    }
}
