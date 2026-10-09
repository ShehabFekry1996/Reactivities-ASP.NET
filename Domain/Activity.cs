using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Activity
    {
        //Strings are easier to work with than GUIDs,
        //so we will use a string for the Id property.
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Title { get; set; }

        public DateTime Date { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public bool IsCancelled { get; set; }

        public required string City { get; set; }

        public required string Venue { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int ImageIndex { get; set; }

        public ICollection<ActivityAttendee> Attendees { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
