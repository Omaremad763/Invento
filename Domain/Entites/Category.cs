// Domain/Categories/Category.cs
namespace Domain.Entites;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    private Category() { }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
