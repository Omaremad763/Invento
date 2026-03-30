namespace Domain.ReadOnlyObjects
{
    public class DashboardWidgetMetric
    {
        public string Name { get; init; } = default!;
        public decimal Value { get; init; }
        public string Unit { get; init; } = default!;
    }
}