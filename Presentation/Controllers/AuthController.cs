using Application.CQRS;
using Application.DTOS.Auth_DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var sending = await _mediator.Send(new RegisterUserCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDTO dto)
        {
            var sending = await _mediator.Send(new ConfirmEmailCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var sending = await _mediator.Send(new LoginCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }

        //[HttpPost("GoogleAuth")]
        //public async Task<IActionResult> GoogleAuth(AuthByGoogleDTO dto)
        //{
        //    var sending = await _mediator.Send(new GoogleAuthCommand(dto));

        //    var response = ApiResponse.Success(sending);
        //    return Ok(response);
        //}

        [HttpPost("GithubAuth")]
        public async Task<IActionResult> GithubAuth(ExternalAuthDTO ExternalAuthDTO)
        {
            var sending = await _mediator.Send(new ExternalAuthCommand(ExternalAuthDTO));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }
    }
}
