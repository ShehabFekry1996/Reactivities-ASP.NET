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

        [HttpPut("{photoId}/setMain")]
        public async Task<ActionResult> SetMainPhoto(string photoId)
        {
            return HandleResult(await Mediator.Send(new SetMainPhoto.Command { PhotoId = photoId }));
        }
    }
}
