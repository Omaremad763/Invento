// Domain/Categories/Category.cs
namespace Domain.Entites;

public class Category:BaseEntity
{
    public Guid Id { get; private set; }
    public string CategoryName { get; private set; } = default!;
    public bool IsDeleted { get; set; }

    private Category() { }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        CategoryName = name;
    }
}
