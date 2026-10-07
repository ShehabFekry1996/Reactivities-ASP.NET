using Application.Activities.Commands;
using Application.Activities.DTOS;
using Application.Activities.Queries;
using Application.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseAPIController
    {

        //It's only service is to receive and respond to HTTP requests
        //The validation and other logic are related to Application 
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> GetActivities()
        {
            // THE API Controllers are thin and don't know what's going on .. on the application layer
            return await Mediator.Send(new GetActivityList.Query());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivityDetail(string id)
        {
            return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
        {
            return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = activityDto }));
        }
        [HttpPut("{id}")]
        [Authorize(Policy="IsActivityHost")]
        public async Task<ActionResult<Unit>> EditActivity(string id,EditActivityDto activity)
        {
           activity.Id = id;
           return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = activity }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> DeleteActivity(string id)
        {
           return HandleResult(await Mediator.Send(new DeleteActivity.Command { Id = id }));
        }


        [HttpPost("{id}/attend")]
        public async Task<ActionResult> Attend(string id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }
    }
}
