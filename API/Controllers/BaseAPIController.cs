using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator
            ??= HttpContext.RequestServices.GetService<IMediator>()
            ?? throw new InvalidOperationException("I Mediator service in unavailable");
    }
}