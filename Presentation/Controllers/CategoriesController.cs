using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
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

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddSupplierCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); ;
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpDelete("DeleteCateogry/{id}")]
        public async Task<IActionResult> DeleteCateogry(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            var response = ApiResponse.Success();
            return Ok(response);
        }

    }
}
