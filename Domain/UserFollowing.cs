using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class UserFollowing
    {
        public required string ObserverId { get; set; }

        public User Observer { get; set; } = null!; // Follower 


        public User Target { get; set; } = null!;

        public required string TargetId { get; set; } // Followee
    }
}
