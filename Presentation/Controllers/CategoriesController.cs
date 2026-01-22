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

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddProduct(AddCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); ;
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> Update(UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpDelete("DeleteCateogry/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            var response = ApiResponse.Success();
            return Ok(response);
        }

    }
}
