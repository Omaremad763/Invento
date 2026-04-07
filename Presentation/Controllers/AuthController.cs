using Application.CQRS;
using Application.DTOS.Auth_DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private static readonly List<string> _staticCache = new List<string>();
        public AuthController(IMediator mediator) => _mediator = mediator;
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
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDTO dto)
        {
            var sending = await _mediator.Send(new ConfirmEmailCommand(dto));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }


        [EnableRateLimiting("auth_policy")]
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
        [EnableRateLimiting("auth_policy")]
        [HttpPost("GithubAuth")]
        public async Task<IActionResult> GithubAuth(ExternalAuthDTO ExternalAuthDTO)
        {
            var sending = await _mediator.Send(new ExternalAuthCommand(ExternalAuthDTO));

            var response = ApiResponse.Success(sending);
            return Ok(response);
        }


        [HttpGet("disaster")]
        public IActionResult GetDisaster()
        {
            // 1. Memory Leak: بنملا List ثابتة ومش بنمسحها أبداً
            for (int i = 0; i < 10000; i++)
            {
                _staticCache.Add($"Data line {i} for tenant {Guid.NewGuid()}");
            }

            // 2. Thread Blocking (الكارثة الأكبر): بنوقف العامل يدوي بدل async
            Thread.Sleep(5000);

            // 3. CPU Burner: عملية حسابية ملهاش لازمة بتاخد وقت
            var result = Enumerable.Range(1, 1000000).Select(n => Math.Sqrt(n)).ToList();

            return Ok("Done with Disaster");
        }

    }
}
