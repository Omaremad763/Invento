using Domain.Entites;

namespace Infrastructure.Persistence
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var electronics = new Category("Electronics");
                var accessories = new Category("Accessories");

                context.Categories.AddRange(electronics, accessories);
                context.SaveChanges();

                var laptop = new Product("Laptop Dell XPS", "LAP-DELL-001", 1500, electronics.Id);
                var mouse = new Product("Logitech Mouse", "MOU-LOGI-001", 40, accessories.Id);

                context.Products.AddRange(laptop, mouse);
                context.SaveChanges();

                context.StockTransactions.AddRange(
                    new StockTransaction(laptop.Id, 100, StockTransactionTypeEnum.Purchase),
                    new StockTransaction(mouse.Id, 200, StockTransactionTypeEnum.Purchase)
                );
                context.SaveChanges();
            }
            // suppliers was missing in the seeder
            if (!context.Suppliers.Any())
            {
                var suppliers = new List<Supplier>
                {
                    new Supplier("Dell Electronics", "contact@dell.com", "01010000001"),
                    new Supplier("Logitech Inc.", "support@logitech.com", "01010000002"),
                    new Supplier("HP Supplies", "info@hp.com", "01010000003")
                };
                context.Suppliers.AddRange(suppliers);
                context.SaveChanges();
            }
        }
    }
}