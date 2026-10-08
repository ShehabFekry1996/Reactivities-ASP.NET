using Application.Profiles.Commands;
using Application.Profiles.Queries;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseAPIController 
    {

        [HttpPost("add-photo")]
        public async Task<ActionResult> AddPhoto(IFormFile file)
        {
          return HandleResult(await Mediator.Send(new AddPhoto.Command { File = file }));
        }

        [HttpGet]
        public async Task<ActionResult> GetProfiles(string? search)
        {
            return HandleResult(await Mediator.Send(new GetProfiles.Query { Search = search }));
        }

        [HttpGet("{userId}/activities")]
        public async Task<ActionResult> GetUserActivities(string userId, string filter = "future")
        {
            return HandleResult(await Mediator.Send(new GetUserActivities.Query { UserId = userId, Filter = filter }));
        }

        [HttpGet("{userId}/photos")]
        public async Task<ActionResult> GetProfilePhotos(string userId)
        {
            return HandleResult(await Mediator.Send(new GetProfilePhotos.Query { UserId = userId }));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetProfile(string userId)
        {
            return HandleResult(await Mediator.Send(new GetProfile.Query { UserId = userId }));
        }

        [HttpDelete("{photoId}/photos")]
        public async Task<ActionResult> DeletePhoto(string photoId)
        {
            return HandleResult(await Mediator.Send(new DeletePhoto.Command { PhotoId = photoId }));
        }

        [HttpPut("{photoId}/set-main-photo")]
        public async Task<ActionResult> SetMainPhoto(string photoId)
        {
            return HandleResult(await Mediator.Send(new SetMainPhoto.Command { PhotoId = photoId }));
        }

        [HttpPost("{userId}/follow")]
        public async Task<ActionResult> FollowUser(string userId)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command { TargetUserId = userId }));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProfile(EditProfile.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        [HttpGet("{userId}/follow-list")]
        public async Task<ActionResult> GetFollowings(string userId,string predicate)
        {
            return HandleResult(await Mediator.Send(new GetFollowings.Query { UserId = userId ,Predicate = predicate}));
        }
    }
}
