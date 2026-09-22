using Application.Activities.Commands;
using Application.Activities.Queries;
using Application.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseAPIController
    {
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // THE API Controllers are thin and don't know what's going on .. on the application layer
            return await Mediator.Send(new GetActivityList.Query());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityDetail(string id)
        {
            return await Mediator.Send(new GetActivityDetails.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(Activity activity)
        {
            return await Mediator.Send(new CreateActivity.Command { Activity = activity });
        }
        [HttpPut]
        public async Task<ActionResult> EditActivity(Activity activity)
        {
            await Mediator.Send(new EditActivity.Command { Activity = activity });
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            await Mediator.Send(new DeleteActivity.Command { Id = id });
            return Ok();
        }
    }
}
