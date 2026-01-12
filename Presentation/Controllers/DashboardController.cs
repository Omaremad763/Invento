using Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

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

        return Ok(result);
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts(
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetTopProductsQuery(limit),
            cancellationToken);

        return Ok(result);
    }
}
