using Application.Core;

namespace Application.Activities.Queries
{
    public class ActivityParams : PaginationParams<DateTime?>
    {
        public string? Filter { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public string SortOrder { get; set; } = "asc";

        public string? Category { get; set; }

        public string? Search { get; set; }
    }
}
