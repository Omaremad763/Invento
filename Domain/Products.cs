// Domain/Products/Product.cs
using Invento.Domain.Categories;

namespace Invento.Domain.Products;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string SKU { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public decimal Price { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    private Product() { } // EF

    public Product(string name, string sku, decimal price, Guid categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        SKU = sku;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = 0;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        StockQuantity += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        if (StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock");

        StockQuantity -= quantity;
    }
}
