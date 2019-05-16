using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string menuChoice;
                do
                {

                    var db = new Context();

                    Console.Clear();
                    Console.WriteLine("<< MAIN MENU >>");
                    Console.WriteLine("1) Product");
                    Console.WriteLine("2) Category");
                    Console.WriteLine("3) Exit");
                    menuChoice = Console.ReadLine();
                    logger.Info("Option {menuChoice} selected", menuChoice);
                    Console.Clear();
                    

                    if (menuChoice == "1")
                    {
                        String choice;
                        do
                        {

                            Console.WriteLine("<< PRODUCTS MENU >>");
                            Console.WriteLine("1) Add Product");
                            Console.WriteLine("2) Edit Product");
                            Console.WriteLine("3) Delete Product");
                            Console.WriteLine("4) View Products");
                            Console.WriteLine("5) Back To Main Menu");
                            choice = Console.ReadLine();
                            logger.Info("Option {choice} selected", choice);
                            Console.Clear();
                            

                            if (choice == "1")
                            {
                                Console.WriteLine("Enter The New Product's Name:");
                                var product = new Product { ProductName = Console.ReadLine() };
                                
                                var query = db.Suppliers.OrderBy(b => b.SupplierId);
                                var maxSupplier = query.Count(); 

                                foreach (var item in query)
                                {
                                    Console.WriteLine(item.SupplierId + ") " + item.CompanyName);
                                }

                                string supplierId;
                                var supplierIsValid = false;
                                while(supplierIsValid == false)
                                {
                                    Console.WriteLine("\nEnter The New Product's Supplier:\n");
                                    Console.WriteLine("(Use the list above)");
                                    supplierId = Console.ReadLine();
                                    if(Convert.ToInt32(supplierId) > 0 && Convert.ToInt32(supplierId) < maxSupplier)
                                    {
                                        supplierIsValid = true;
                                        product.SupplierId = Convert.ToInt32(supplierId);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please Enter Valid Supplier ID");
                                    }
                                }

                                Console.Write("\nEnter The Quantity Per Unit:");
                                product.QuantityPerUnit = Console.ReadLine();
                                Console.Write("\nEnter The Unit Price:");
                                product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
                                Console.Write("\nEnter The Units In Stock:");
                                product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
                                Console.Write("\nEnter The Units On Order:");
                                product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
                                Console.Write("\nEnter The Reorder Level:");
                                product.ReorderLevel = Convert.ToInt16(Console.ReadLine());

                                var discontinuedIsValid = false;
                                while (discontinuedIsValid == false)
                                {
                                    Console.Write("\nEnter If The Product Is Discontinued:");
                                    Console.WriteLine("(0 for active, 1 for discontinued)");
                                    var discontinuedString = Console.ReadLine();
                                    if (discontinuedString == "0")
                                    {
                                        product.Discontinued = false;
                                        discontinuedIsValid = true;
                                    }
                                    else if (discontinuedString == "1")
                                    {
                                        product.Discontinued = true;
                                        discontinuedIsValid = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please Enter A Valid Response");
                                        logger.Info("Invalid Response Chosen - " + discontinuedString);
                                    }
                                }

                                var supplierQuery = db.Categories.OrderBy(b => b.CategoryId);
                                var maxCategory = supplierQuery.Count();

                                foreach (var item in supplierQuery)
                                {
                                    Console.WriteLine(item.CategoryId + ") " + item.CategoryName);
                                }

                                string categoryId;
                                var categoryIsValid = false;
                                while (categoryIsValid == false)
                                {
                                    Console.WriteLine("\nEnter The New Product's Category:\n");
                                    Console.WriteLine("(Use the list above)");
                                    categoryId = Console.ReadLine();
                                    if (Convert.ToInt32(categoryId) > 0 && Convert.ToInt32(categoryId) < maxSupplier)
                                    {
                                        categoryIsValid = true;
                                        product.CategoryId = Convert.ToInt32(categoryId);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please Enter Valid Category ID");
                                    }
                                }

                                ValidationContext context = new ValidationContext(product, null, null);
                                List<ValidationResult> results = new List<ValidationResult>();

                                var isValid = Validator.TryValidateObject(product, context, results, true);
                                if (isValid)
                                {
                                    // check for unique name
                                    if (db.Products.Any(b => b.ProductName == product.ProductName))
                                    {
                                        // generate validation error
                                        isValid = false;
                                        results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
                                    }
                                    else
                                    {
                                        logger.Info("Validation passed");
                                        // save blog to db
                                        db.AddProduct(product);
                                        logger.Info("Product added - {name}", product.ProductName);
                                    }
                                }
                                if (!isValid)
                                {
                                    foreach (var result in results)
                                    {
                                        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                    }
                                }
                            }
                            else if (choice == "2")
                            {                                
                                var product = GetProduct(db);
                                if (product != null)
                                {
                                    // input post
                                    Product UpdatedProduct = InputProduct(db);
                                    if (UpdatedProduct != null)
                                    {
                                        UpdatedProduct.ProductId = product.ProductId;
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productid}) updated", UpdatedProduct.ProductId);
                                    }
                                }
                            }
                            else if (choice == "3")
                            {
                                Console.WriteLine("Choose The Product To Delete:");
                                var product = GetProduct(db);
                                if (product != null)
                                {
                                    db.DeleteProduct(product);
                                    logger.Info("Post (id: {productid}) deleted", product.ProductId);
                                }

                                Console.WriteLine("");
                                Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                Console.ReadLine();
                                Console.Clear();

                            }
                            else if (choice == "4")
                            {
                                string viewChoice;
                                do
                                {
                                    Console.WriteLine("<< VIEW MENU >>");
                                    Console.WriteLine("1) View All Products");
                                    Console.WriteLine("2) View Active Products");
                                    Console.WriteLine("3) View Discontinued Products");
                                    Console.WriteLine("4) View Specific Product");
                                    Console.WriteLine("5) Back To Products Menu");
                                    viewChoice = Console.ReadLine();
                                    logger.Info("Option {choice} selected", choice);
                                    Console.Clear();

                                    if (viewChoice == "1")
                                    {
                                        var query = db.Products.OrderBy(b => b.ProductName);

                                        foreach (var item in query)
                                        {
                                            if (item.Discontinued == false)
                                            {
                                                Console.WriteLine(item.ProductName);
                                            }
                                            else
                                            {
                                                Console.WriteLine(item.ProductName + " *DISCONTINUED*");
                                            }
                                        }
                                        Console.WriteLine("");
                                        Console.WriteLine($"{query.Count()} - Products Returned");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();
                                    }
                                    else if(viewChoice == "2")
                                    {
                                        var query = db.Products.OrderBy(b => b.ProductName).Where(b => b.Discontinued == false);

                                        foreach (var item in query)
                                        {
                                            Console.WriteLine(item.ProductName);
                                        }

                                        Console.WriteLine("");
                                        Console.WriteLine($"{query.Count()} - Active Products Returned");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();
                                    }
                                    else if (viewChoice == "3")
                                    {
                                        var query = db.Products.OrderBy(b => b.ProductName).Where(b => b.Discontinued == true);

                                        foreach (var item in query)
                                        {
                                            Console.WriteLine(item.ProductName);
                                        }

                                        Console.WriteLine("");
                                        Console.WriteLine($"{query.Count()} - Discontinued Products Returned");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();
                                    }
                                    else if (viewChoice == "4")
                                    {
                                        var query = db.Products.OrderBy(b => b.ProductId);
                                        foreach (var item in query)
                                        {
                                            if (item.Discontinued == false)
                                            {
                                                Console.WriteLine(item.ProductId + ") " + item.ProductName);
                                            }
                                            else
                                            {
                                                Console.WriteLine(item.ProductId + ") " + item.ProductName + " *DISCONTINUED*");
                                            }
                                        }

                                        var maxProduct = query.Count();
                                        string specificProductId;
                                        var waitingForValidAnswer = true;
                                        while (waitingForValidAnswer == true)
                                        {
                                            Console.WriteLine("\nEnter In Specific Product ID To View:");
                                            specificProductId = Console.ReadLine();
                                            if(Convert.ToInt32(specificProductId) > 0 && Convert.ToInt32(specificProductId) <= maxProduct)
                                            {
                                                var convertedSpecificProductId = Convert.ToInt32(specificProductId);
                                                waitingForValidAnswer = false;
                                                var specificQuery = db.Products.Where(b => b.ProductId == convertedSpecificProductId);
                                                foreach (var item in specificQuery)
                                                {
                                                    Console.WriteLine("");
                                                    Console.WriteLine("    Product ID: " + item.ProductId);
                                                    Console.WriteLine("  Product Name: " + item.ProductName);
                                                    Console.WriteLine("   Supplier ID: " + item.SupplierId);
                                                    Console.WriteLine("    Unit Price: " + item.UnitPrice);
                                                    Console.WriteLine("Units In Stock: " + item.UnitsInStock);
                                                    Console.WriteLine("Units On Order: " + item.UnitsOnOrder);
                                                    Console.WriteLine(" Reorder Level: " + item.ReorderLevel);
                                                    Console.WriteLine("  Discontinued: " + item.Discontinued);
                                                }
                                                Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                                Console.ReadLine();
                                                Console.Clear();
                                            }
                                            else
                                            {
                                                Console.WriteLine("<< PLEASE ENTER VALID RESPONSE >> ");
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if(viewChoice != "5")
                                        {
                                            Console.WriteLine("<< PLEASE ENTER VALID RESPONSE >>");
                                        }
                                    }
                                } while (viewChoice != "5");
                            }
                            else
                            {

                            }

                        } while (choice != "5");
                    }
                    else if (menuChoice == "2")
                    {
                        String choice;
                        do
                        {
                            Console.WriteLine("<< CATEGORIES MENU >>");
                            Console.WriteLine("1) Add Category");
                            Console.WriteLine("2) Edit Category");
                            Console.WriteLine("3) Delete Category");
                            Console.WriteLine("4) View All Categories");
                            Console.WriteLine("5) Back To Main Menu");
                            choice = Console.ReadLine();
                            Console.Clear();
                            logger.Info("Option {choice} selected", choice);

                            if (choice == "1")
                            {
                                Console.WriteLine("Enter The New Category's Name:");
                                var category = new Category { CategoryName = Console.ReadLine() };
                                Console.WriteLine("Enter The New Category's Description:");
                                category.Description = Console.ReadLine();
                                category.CategoryId = db.Categories.Count() + 1;

                                ValidationContext context = new ValidationContext(category, null, null);
                                List<ValidationResult> results = new List<ValidationResult>();

                                var isValid = Validator.TryValidateObject(category, context, results, true);
                                if (isValid)
                                {
                                    if (db.Products.Any(b => b.ProductName == category.CategoryName))
                                    {
                                        isValid = false;
                                        results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
                                    }
                                    else
                                    {
                                        logger.Info("Validation passed");
                                        db.AddCategory(category);
                                        logger.Info("Category added - {name}", category.CategoryName);
                                    }
                                }
                                if (!isValid)
                                {
                                    foreach (var result in results)
                                    {
                                        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                    }
                                }



                            }
                            else if (choice == "2")
                            {
                                var categories = db.Categories.Include("Products").OrderBy(b => b.CategoryName);
                                var maxCategories = categories.Count();

                                foreach (var item in categories)
                                {
                                    Console.WriteLine(item.CategoryId + ") " + item.CategoryName);
                                }

                                var category = new Category();
                                string categoryChoice;
                                var categoryQuestion = true;
                                while (categoryQuestion == true)
                                {
                                    Console.WriteLine("Enter The Category ID To Edit");
                                    categoryChoice = Console.ReadLine();
                                    if(Convert.ToInt32(categoryChoice) > 0 && Convert.ToInt32(categoryChoice) <= maxCategories)
                                    {
                                        category.CategoryId = Convert.ToInt32(categoryChoice);
                                        categoryQuestion = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("<<Please Enter Valid Category ID>>");
                                    }
                                }

                                Console.WriteLine("Enter The New Category's Name:");
                                category.CategoryName = Console.ReadLine();
                                Console.WriteLine("Enter The New Category's Description:");
                                category.Description = Console.ReadLine();
                                ValidationContext context = new ValidationContext(category, null, null);
                                List<ValidationResult> results = new List<ValidationResult>();
                                var isValid = Validator.TryValidateObject(category, context, results, true);
                                if (isValid)
                                {
                                    db.EditCategory(category);
                                }
                                else
                                {
                                    foreach (var result in results)
                                    {
                                        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                    }
                                }
                            }
                            else if (choice == "3")
                            {
                                Console.WriteLine("Choose The Category To Delete:");
                                var category = GetCategory(db);
                                //Console.WriteLine(category.CategoryId);
                                if (category != null)
                                {

                                    /*var products = db.Categories.Include("Products").Where(b => b.CategoryId == Products.CategoryId);
                                    foreach (var item in products)
                                    {
                                        var product = db.Products.FirstOrDefault(p => p.ProductId == category.CategoryId);
                                        db.DeleteProduct(product);
                                    }
                                    */
                                    db.DeleteCategory(category);
                                    logger.Info("Category (id: {categoryid}) deleted", category.CategoryId);

                                    Console.WriteLine("");
                                    Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                    Console.ReadLine();
                                    Console.Clear();

                                }


                            }
                            else if (choice == "4")
                            {
                                string viewChoice;
                                do
                                {
                                    Console.WriteLine("<< VIEW MENU >>");
                                    Console.WriteLine("1) View All Categories");
                                    Console.WriteLine("2) View Active Category Products");
                                    Console.WriteLine("3) View Specific Active Category Products");
                                    Console.WriteLine("4) Back To Categories Menu");
                                    viewChoice = Console.ReadLine();
                                    logger.Info("Option {choice} selected", choice);
                                    Console.Clear();

                                    if (viewChoice == "1")
                                    {
                                        var query = db.Categories.OrderBy(b => b.CategoryName);

                                        Console.WriteLine($"{query.Count()} Categories returned");
                                        foreach (var item in query)
                                        {
                                            Console.WriteLine(item.CategoryName);
                                            Console.WriteLine("   " + item.Description + "\n");
                                        }
                                        Console.WriteLine("");
                                        Console.WriteLine($"{query.Count()} - Categories Returned");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();
                                    }
                                    else if (viewChoice == "2")
                                    {
                                        var categories = db.Categories.Include("Products").OrderBy(b => b.CategoryName);
                                        foreach (Category b in categories)
                                        {
                                            Console.WriteLine(b.CategoryName);
                                            if (b.Products.Count() == 0)
                                            {
                                                Console.WriteLine($"  <no posts>");
                                            }
                                            else
                                            {
                                                foreach (Product p in b.Products)
                                                {
                                                    if (p.Discontinued == false)
                                                    {
                                                        Console.WriteLine($"  {p.ProductId}) {p.ProductName}");
                                                    }
                                                }
                                            }
                                        }

                                        Console.WriteLine("");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();

                                    }
                                    else if (viewChoice == "3")
                                    {
                                        var categories = db.Categories.Include("Products").OrderBy(b => b.CategoryName);
                                        var maxCategories = categories.Count();
                                        var categoryQuestion = true;
                                        string categoryChoice;
                                        while (categoryQuestion == true)
                                        {
                                            Console.WriteLine("Which Category Would You Like To View?");
                                            categoryChoice = Console.ReadLine();
                                            if (Convert.ToInt32(categoryChoice) > 0 && Convert.ToInt32(categoryChoice) <= maxCategories)
                                            {
                                                var categoryChoiceForReal = Convert.ToInt32(categoryChoice);
                                                categoryQuestion = false;
                                                categories = db.Categories.Include("Products").Where(b => b.CategoryId == categoryChoiceForReal).OrderBy(b => b.CategoryName);
                                            }
                                            else
                                            {
                                                Console.WriteLine("<<Please Enter Valid Response>>");
                                            }
                                        }

                                        foreach (Category b in categories)
                                        {
                                            Console.WriteLine(b.CategoryName);
                                            if (b.Products.Count() == 0)
                                            {
                                                Console.WriteLine($"  <no posts>");
                                            }
                                            else
                                            {
                                                foreach (Product p in b.Products)
                                                {
                                                    if (p.Discontinued == false)
                                                    {
                                                        Console.WriteLine($"  {p.ProductId}) {p.ProductName}");
                                                    }
                                                }
                                            }
                                        }

                                        Console.WriteLine("");
                                        Console.WriteLine("\n<< PRESS ENTER TO CONTINUE >>");
                                        Console.ReadLine();
                                        Console.Clear();
                                    }
                                    else
                                    {
                                        if (viewChoice != "4")
                                        {
                                            Console.WriteLine("<< PLEASE ENTER VALID RESPONSE >>");
                                        }
                                    }
                                } while (viewChoice != "4");
                            }
                            else
                            {

                            }

                        } while (choice != "5");
                    }
                } while (menuChoice != "3");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Product InputProduct(Context db)
        {
            Console.WriteLine("Enter The New Product's Name:");
            var product = new Product { ProductName = Console.ReadLine() };

            var query = db.Suppliers.OrderBy(b => b.SupplierId);
            var maxSupplier = query.Count();

            foreach (var item in query)
            {
                Console.WriteLine(item.SupplierId + ") " + item.CompanyName);
            }

            string supplierId;
            var supplierIsValid = false;
            while (supplierIsValid == false)
            {
                Console.WriteLine("\nEnter The New Product's Supplier:\n");
                Console.WriteLine("(Use the list above)");
                supplierId = Console.ReadLine();
                if (Convert.ToInt32(supplierId) > 0 && Convert.ToInt32(supplierId) < maxSupplier)
                {
                    supplierIsValid = true;
                    product.SupplierId = Convert.ToInt32(supplierId);
                }
                else
                {
                    Console.WriteLine("Please Enter Valid Supplier ID");
                }
            }

            Console.Write("\nEnter The Quantity Per Unit:");
            product.QuantityPerUnit = Console.ReadLine();
            Console.Write("\nEnter The Unit Price:");
            product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
            Console.Write("\nEnter The Units In Stock:");
            product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter The Units On Order:");
            product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter The Reorder Level:");
            product.ReorderLevel = Convert.ToInt16(Console.ReadLine());

            var discontinuedIsValid = false;
            while (discontinuedIsValid == false)
            {
                Console.Write("\nEnter If The Product Is Discontinued:");
                Console.WriteLine("(0 for active, 1 for discontinued)");
                var discontinuedString = Console.ReadLine();
                if (discontinuedString == "0")
                {
                    product.Discontinued = false;
                    discontinuedIsValid = true;
                }
                else if (discontinuedString == "1")
                {
                    product.Discontinued = true;
                    discontinuedIsValid = true;
                }
                else
                {
                    Console.WriteLine("Please Enter A Valid Response");
                    logger.Info("Invalid Response Chosen - " + discontinuedString);
                }
            }

            var supplierQuery = db.Categories.OrderBy(b => b.CategoryId);
            var maxCategory = supplierQuery.Count();

            foreach (var item in supplierQuery)
            {
                Console.WriteLine(item.CategoryId + ") " + item.CategoryName);
            }

            string categoryId;
            var categoryIsValid = false;
            while (categoryIsValid == false)
            {
                Console.WriteLine("\nEnter The New Product's Category:\n");
                Console.WriteLine("(Use the list above)");
                categoryId = Console.ReadLine();
                if (Convert.ToInt32(categoryId) > 0 && Convert.ToInt32(categoryId) < maxSupplier)
                {
                    categoryIsValid = true;
                    product.CategoryId = Convert.ToInt32(categoryId);
                }
                else
                {
                    Console.WriteLine("Please Enter Valid Category ID");
                }
            }

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                return product;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }

        public static Product GetProduct(Context db)
        {
            var categories = db.Categories.Include("Products").OrderBy(b => b.CategoryName);
            foreach (Category b in categories)
            {
                Console.WriteLine(b.CategoryName);
                if (b.Products.Count() == 0)
                {
                    Console.WriteLine($"  <no posts>");
                }
                else
                {
                    foreach (Product p in b.Products)
                    {
                        Console.WriteLine($"  {p.ProductId}) {p.ProductName}");
                    }
                }
            }
            Console.WriteLine("\nEnter The Product Id:");
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Category GetCategory(Context db)
        {
            var categories = db.Categories.OrderBy(b => b.CategoryName);
            foreach (Category b in categories)
            {
                Console.WriteLine(b.CategoryId + ") " + b.CategoryName);
            }
            Console.WriteLine("\nEnter The Category Id:");
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(p => p.CategoryId == CategoryId);
                category.CategoryId = CategoryId;
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;

        }




    }
}















/*
if (choice == "1")
{
// display blogs
var db = new BloggingContext();
var query = db.Blogs.OrderBy(b => b.Name);

Console.WriteLine($"{query.Count()} Blogs returned");
foreach (var item in query)
{
    Console.WriteLine(item.Name);
}
}
else if (choice == "2")
{
// Add blog
Console.Write("Enter a name for a new Blog: ");
var blog = new Blog { Name = Console.ReadLine() };

ValidationContext context = new ValidationContext(blog, null, null);
List<ValidationResult> results = new List<ValidationResult>();

var isValid = Validator.TryValidateObject(blog, context, results, true);
if (isValid)
{
    var db = new BloggingContext();
    // check for unique name
    if (db.Blogs.Any(b => b.Name == blog.Name))
    {
        // generate validation error
        isValid = false;
        results.Add(new ValidationResult("Blog name exists", new string[] { "Name" }));
    }
    else
    {
        logger.Info("Validation passed");
        // save blog to db
        db.AddBlog(blog);
        logger.Info("Blog added - {name}", blog.Name);
    }
}
if (!isValid)
{
    foreach (var result in results)
    {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }
}
}
else if (choice == "3")
{
// Create Post
var db = new BloggingContext();
var query = db.Blogs.OrderBy(b => b.BlogId);

Console.WriteLine("Select the blog you would to post to:");
foreach (var item in query)
{
    Console.WriteLine($"{item.BlogId}) {item.Name}");
}
if (int.TryParse(Console.ReadLine(), out int BlogId))
{
    if (db.Blogs.Any(b => b.BlogId == BlogId))
    {
        Post post = InputPost(db);
        if (post != null)
        {
            post.BlogId = BlogId;
            db.AddPost(post);
            logger.Info("Post added - {title}", post.Title);
        }
    }
    else
    {
        logger.Error("There are no Blogs saved with that Id");
    }
}
else
{
    logger.Error("Invalid Blog Id");
}
}
else if (choice == "4")
{
// Display Posts
var db = new BloggingContext();
var query = db.Blogs.OrderBy(b => b.BlogId);
Console.WriteLine("Select the blog's posts to display:");
Console.WriteLine("0) Posts from all blogs");
foreach (var item in query)
{
    Console.WriteLine($"{item.BlogId}) Posts from {item.Name}");
}

if (int.TryParse(Console.ReadLine(), out int BlogId))
{
    IEnumerable<Post> Posts;
    if (BlogId != 0 && db.Blogs.Count(b => b.BlogId == BlogId) == 0)
    {
        logger.Error("There are no Blogs saved with that Id");
    }
    else
    {
        // display posts from all blogs
        Posts = db.Posts.OrderBy(p => p.Title);
        if (BlogId == 0)
        {
            // display all posts from all blogs
            Posts = db.Posts.OrderBy(p => p.Title);
        }
        else
        {
            // display post from selected blog
            Posts = db.Posts.Where(p => p.BlogId == BlogId).OrderBy(p => p.Title);
        }
        Console.WriteLine($"{Posts.Count()} post(s) returned");
        foreach (var item in Posts)
        {
            Console.WriteLine($"Blog: {item.Blog.Name}\nTitle: {item.Title}\nContent: {item.Content}\n");
        }
    }
}
else
{
    logger.Error("Invalid Blog Id");
}
}
else if (choice == "5")
{
// delete post
Console.WriteLine("Choose the post to delete:");
var db = new BloggingContext();
var post = GetPost(db);
if (post != null)
{
    db.DeletePost(post);
    logger.Info("Post (id: {postid}) deleted", post.PostId);
}
}
else if (choice == "6")
{
// edit post
Console.WriteLine("Choose the post to edit:");
var db = new BloggingContext();
var post = GetPost(db);
if (post != null)
{
    // input post
    Post UpdatedPost = InputPost(db);
    if (UpdatedPost != null)
    {
        UpdatedPost.PostId = post.PostId;
        db.EditPost(UpdatedPost);
        logger.Info("Post (id: {postid}) updated", UpdatedPost.PostId);
    }
}
}
Console.WriteLine();*/
