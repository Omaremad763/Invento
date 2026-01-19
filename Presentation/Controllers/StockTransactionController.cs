using Application.CQRS;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StockTransactionController(IMediator mediator) => _mediator = mediator;

        [HttpPost("AddStockTransaction")]
        public async Task<IActionResult> AddProduct(AddStockCommand command)
        {
            var result = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); ;
        }
    }
}
