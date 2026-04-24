using ConnectDB.Models;
using System.Linq;

namespace ConnectDB.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User 
                    { 
                        Name = "Admin User", 
                        Email = "admin@example.com", 
                        Password = "123", 
                        Role = "Admin",
                        Phone = "0123456789",
                        Address = "123 Admin St"
                    },
                    new User 
                    { 
                        Name = "Customer User", 
                        Email = "customer@example.com", 
                        Password = "123", 
                        Role = "customer",
                        Phone = "0987654321",
                        Address = "456 Customer Ave"
                    }
                );
                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Áo Nam", Slug = "ao-nam", Description = "Danh mục áo nam", Image = "https://picsum.photos/200/300" },
                    new Category { Name = "Quần Nam", Slug = "quan-nam", Description = "Danh mục quần nam", Image = "https://picsum.photos/200/300" }
                );
                context.SaveChanges();
            }

            if (!context.Brands.Any())
            {
                context.Brands.AddRange(
                    new Brand { Name = "Nike", Description = "Thương hiệu Nike", Logo = "https://picsum.photos/100/100" },
                    new Brand { Name = "Adidas", Description = "Thương hiệu Adidas", Logo = "https://picsum.photos/100/100" }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var cat1 = context.Categories.FirstOrDefault(c => c.Name == "Áo Nam");
                var brand1 = context.Brands.FirstOrDefault(b => b.Name == "Nike");

                if (cat1 != null && brand1 != null)
                {
                    context.Products.AddRange(
                        new Product
                        {
                            Name = "Áo Thun Thể Thao Nike",
                            Slug = "ao-thun-the-thao-nike",
                            Price = 500000,
                            SalePrice = 450000,
                            Description = "Áo thun thoải mái cho mùa hè.",
                            Image = "https://picsum.photos/400/500",
                            Stock = 100,
                            CategoryId = cat1.CategoryId,
                            BrandId = brand1.BrandId
                        },
                        new Product
                        {
                            Name = "Áo Khoác Gió Nam",
                            Slug = "ao-khoac-gio-nam",
                            Price = 1200000,
                            Description = "Áo khoác gió chống nước nhẹ.",
                            Image = "https://picsum.photos/400/501",
                            Stock = 50,
                            CategoryId = cat1.CategoryId,
                            BrandId = brand1.BrandId
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
