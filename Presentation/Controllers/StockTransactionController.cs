using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StockTransactionController(IMediator mediator) => _mediator = mediator;

        [HttpPost("AddStockTransaction")]
        public async Task<IActionResult> AddStockTransaction(AddStockCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); ;
        }

        [HttpGet("GetAlStockTransactions")]
        public async Task<IActionResult> GetAlStockTransactions([FromQuery] ResourceParameters parameters)
        {
            var result = await _mediator.Send(new GetStockssQuery(parameters));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }

        [HttpDelete("DeleteStockTransaction/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteStockTransactionCommand(id));
            var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpGet("GetProductsLookUp")]
        public async Task<IActionResult> GetProductsLookUp()

        {
            var result = await _mediator.Send(new GetProductsLookupQuery());
            var response = ApiResponse.Success(result);
            return Ok(response);
        }
    }
}
