using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invento.Domain.Categories;
using Invento.Domain.Products;
using Invento.Domain.Stock;

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
                    new StockTransaction(laptop.Id, 100, StockTransactionType.Purchase),
                    new StockTransaction(mouse.Id, 200, StockTransactionType.Purchase)
                );

                context.SaveChanges();
            }
        }
    }
}