namespace Domain.ReadOnlyObjects
{
    public class TopProductResult
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = default!;
        public int TotalSoldQuantity { get; init; }
    }
}