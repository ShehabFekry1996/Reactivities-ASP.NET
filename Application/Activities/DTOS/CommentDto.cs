using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.DTOS
{
    public class CommentDto
    {
        public required string Id { get; set; }

        public required string Body { get; set; }

        public DateTime CreatedAt { get; set; } 

        public required string UserId { get; set; }

        public required string DisplayName { get; set; }

        public string? ImageUrl { get; set; }

    }
}
