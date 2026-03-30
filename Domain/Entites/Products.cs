// Domain/Products/Product.cs
namespace Domain.Entites;

public class Product : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string SKU { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public decimal Price { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;
    public bool IsDeleted { get; set; }

    private Product()
    { } // EF

    public Product(string name, string sku, decimal price, Guid categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        SKU = sku;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = 0;
    }

    public void ChangeStock(int quantity, int StoredQuantity, StockTransactionTypeEnum Type)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive");

        if (Type == StockTransactionTypeEnum.Sale)
        {
            if (StockQuantity < quantity) throw new InvalidOperationException("Insufficient stock");
            StockQuantity = StoredQuantity - quantity;
        }
        else if (Type == StockTransactionTypeEnum.Purchase)
        {
            StockQuantity = StoredQuantity + quantity;
        }
    }
}