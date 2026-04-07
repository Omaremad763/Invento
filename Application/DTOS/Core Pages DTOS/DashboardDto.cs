namespace Application.DTOS
{
    public record DashboardMetricDto(
       string Name,
       decimal Value,
       string Unit);
    public record TopProductDto(
        Guid ProductId,
        string ProductName,
        int TotalSoldQuantity);
}