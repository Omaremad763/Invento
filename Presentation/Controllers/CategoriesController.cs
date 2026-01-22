using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController:ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("GetAlCategories")]
        public async Task<IActionResult> GetAlCategories([FromQuery] ResourceParameters parameters)
        {
            var result = await _mediator.Send(new GetCategoriesQuery(parameters));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }
    }
}
