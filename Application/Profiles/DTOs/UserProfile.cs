using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.DTOs
{
    public class UserProfile
    {
        public required string Id { get; set; }

        public required string DisplayName { get; set; }

        public string? Bio { get; set; }

        public string? ImageUrl { get; set; }
    }
}
