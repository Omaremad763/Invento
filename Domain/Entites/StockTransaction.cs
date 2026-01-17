// Domain/Stock/StockTransaction.cs
namespace Domain.Entites;

public class StockTransaction:BaseEntity
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;

    public int Quantity { get; private set; }
    public StockTransactionTypeEnum StockTransactionType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    private StockTransaction() { }

    public StockTransaction(Guid productId, int quantity, StockTransactionTypeEnum type)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        StockTransactionType = type;
        CreatedAt = DateTime.UtcNow;
    }
}
