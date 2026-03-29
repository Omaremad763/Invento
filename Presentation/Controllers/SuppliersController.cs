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
    public class SuppliersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("GetAlSuppliers")]
        public async Task<IActionResult> GetAlSuppliers([FromQuery] ResourceParameters parameters)
        {
            var result = await _mediator.Send(new GetSuppliersQuery(parameters));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }

        [HttpPost("AddSupplier")]
        public async Task<IActionResult> AddSupplier(AddSupplierCommand command)
        {
            _ = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response); 
        }

        [HttpPut("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier(UpdateSupplierCommand command)
        {
            _ = await _mediator.Send(command);
            var response = ApiResponse.Success();
            return Ok(response);
        }

        [HttpDelete("DeleteSupplier/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            _ = await _mediator.Send(new DeleteSupplierCommand(id));
            var response = ApiResponse.Success();
            return Ok(response);
        }

    }
}
