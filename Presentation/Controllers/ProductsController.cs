using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;


    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("GetAlProducts")]
        public async Task<IActionResult> GetAlProducts( [FromQuery]GetProductsQuery Query)
        {
            var result = await _mediator.Send(Query);
            var response = ApiResponse.Success(result);
        return Ok(response);
        }

        [HttpGet("GetProductByID{ID}")]
        public async Task<IActionResult> GetProductByID(Guid ID)
        {
            var result = await _mediator.Send(new GetProductByIDQuery(ID));
        var response = ApiResponse.Success(result);
        return Ok(response);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); ;
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpDelete("Delete{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
             var response = ApiResponse.Success();
             return Ok(response);
        }
    }

