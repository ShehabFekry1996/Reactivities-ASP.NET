using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController(AppDBContext context) : BaseAPIController
    {
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // To make this more scalable
            // servers are multi threaded, so we can use async/await to free
            // up the thread while waiting for the database query to complete
            //but when the thread receives too many requests at the same time it can become a bottleneck
            //, so we can use async/await to
            //free up the thread while waiting for the database query to complete
            return await context.Activities.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityDetail(string id)
        {
            var activity = await context.Activities.FindAsync(id);
            if (activity == null) return NotFound();
            return activity;
        }

    }
}
