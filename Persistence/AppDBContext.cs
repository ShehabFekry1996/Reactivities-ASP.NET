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

        public required DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public required DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // keep it 
            builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));
            builder.Entity<ActivityAttendee>()
                .HasOne(a => a.User)
                .WithMany(x => x.Activities)
                .HasForeignKey(a => a.UserId);
            builder.Entity<ActivityAttendee>()
              .HasOne(a => a.Activity)
              .WithMany(x=>x.Attendees)
              .HasForeignKey(a => a.ActivityId);
        }
    }
}
