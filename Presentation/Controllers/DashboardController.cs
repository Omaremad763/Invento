using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation;

namespace Presentation.Controllers;

    [Authorize]

    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

    [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetDashboardMetricsQuery(),
                cancellationToken);
            var response = ApiResponse.Success(result);
            return Ok(response);
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetTopProductsQuery(limit),
                cancellationToken);

            var response = ApiResponse.Success(result);
            return Ok(response);
        }


    }
