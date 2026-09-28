using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Activities.DTOS
{
    public class CreateActivityDto()
    {
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public bool IsCancelled { get; set; } 
        public string City { get; set; } = "";
        public string Venue { get; set; } = "";
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
    