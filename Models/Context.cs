using System;
using System.Data.Entity;

namespace BlogsConsole.Models
{
    public class Context : DbContext
    {
        public Context() : base("name=Context") { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public void AddCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            this.Products.Add(product);
            this.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            this.Products.Remove(product);
            this.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            Console.WriteLine("1:");
            this.Categories.Remove(category);
            Console.WriteLine("2:");
            this.SaveChanges();
            Console.WriteLine("3:");
        }

        public void EditProduct(Product UpdatedProduct)
        {
            Product product = this.Products.Find(UpdatedProduct.ProductId);
            product.ProductName = UpdatedProduct.ProductName;
            product.SupplierId = UpdatedProduct.SupplierId;
            product.QuantityPerUnit = UpdatedProduct.QuantityPerUnit;
            product.UnitPrice = UpdatedProduct.UnitPrice;
            product.UnitsInStock = UpdatedProduct.UnitsInStock;
            product.UnitsOnOrder = UpdatedProduct.UnitsOnOrder;
            product.ReorderLevel = UpdatedProduct.ReorderLevel;
            product.Discontinued = UpdatedProduct.Discontinued;
            product.CategoryId = UpdatedProduct.CategoryId;
            this.SaveChanges();
    }

        public void EditCategory(Category UpdatedCategory)
        {
            Category category = this.Categories.Find(UpdatedCategory.CategoryId);
            category.CategoryName = UpdatedCategory.CategoryName;
            category.Description = UpdatedCategory.Description;
            this.SaveChanges();
        }
    }
}
