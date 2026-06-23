using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]

[Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

    [HttpGet("GetAlProducts")]
        public async Task<IActionResult> GetAlProducts( [FromQuery] ResourceParameters parameters)
        {
            var result = await _mediator.Send(new GetProductsQuery(parameters));
            var response = ApiResponse.Success(result);
             return Ok(response);
        }

        [HttpGet("GetProductByID/{ID}")]
        public async Task<IActionResult> GetProductByID(Guid ID)
        {
            var result = await _mediator.Send(new GetProductByIDQuery(ID));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductCommand command)
        {
        _ = await _mediator.Send(command);
        var response = ApiResponse.Success();
            return Ok(response); 
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
        _ = await _mediator.Send(command);
        var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
        _ = await _mediator.Send(new DeleteProductCommand(id));
        var response = ApiResponse.Success();
             return Ok(response);
        }


}

