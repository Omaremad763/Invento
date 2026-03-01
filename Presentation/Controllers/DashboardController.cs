using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation;
[Authorize]

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

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

        var response =ApiResponse.Success(result);
        return Ok(response);
    }


}
