using Application.CQRS;
using Application.DTOS.Auth_DTOS;

using AspNetCore.ReCaptcha;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator _mediator) : ControllerBase
    {
        [EnableRateLimiting("auth_policy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var sending = await _mediator.Send(new RegisterUserCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto dto)
        {
            var sending = await _mediator.Send(new ConfirmEmailCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }


        [EnableRateLimiting("auth_policy")]
        [HttpPost("Login")]
        [ValidateReCaptcha]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var sending = await _mediator.Send(new LoginCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }

        [EnableRateLimiting("auth_policy")]
        [HttpPost("GithubAuth")]
        public async Task<IActionResult> GithubAuth(ExternalAuthDto ExternalAuthDTO)
        {
            var sending = await _mediator.Send(new ExternalAuthCommand(ExternalAuthDTO));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }


    }
}
