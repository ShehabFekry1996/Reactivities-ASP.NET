using Application.Activities.Commands;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class CommentHub(IMediator mediator) : Hub
    {

        //To Invoke the method we need the client to call the method name in the hub,
        //so we need to make sure that the method name is the same as the one in the client side
        public async Task SendComment(AddComment.Command command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess && result.Value is not null)
            {
                //Important string to what will be sent to the client,
                //it should match the method name in the client side
                await Clients.Group(command.ActivityId).SendAsync("ReceiveComment", result.Value);
            }
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext?.Request.Query["activityId"].ToString();
            if (string.IsNullOrEmpty(activityId)) // we can't add to group if activityId is null or empty
            {
                throw new HubException("ActivityId is required to join the group.");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            var result = await mediator.Send(new GetComments.Query { ActivityId = activityId });
            //Important string to what will be sent to the client,
            //it should match the method name in the client side
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}
