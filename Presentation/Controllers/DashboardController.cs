using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Presentation;

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
        var response = new GlobalApiResponse<IReadOnlyList<DashboardMetricDto>>(result);
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

        GlobalApiResponse<IReadOnlyList<TopProductDto>>? response =new  GlobalApiResponse<IReadOnlyList<TopProductDto>>(result);
        return Ok(response);
    }


}
