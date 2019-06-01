using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //01
            //var usersJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Product Shop - Skeleton\ProductShop\Datasets\users.json");
            //var result = ImportUsers(context, usersJson);

            //02
            //var productsJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Product Shop - Skeleton\ProductShop\Datasets\products.json");
            //var result = ImportProducts(context, productsJson);

            //03
            //var categoriesJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Product Shop - Skeleton\ProductShop\Datasets\categories.json");
            //var result = ImportCategories(context, categoriesJson);

            //04
            //var categoriesProductsJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Product Shop - Skeleton\ProductShop\Datasets\categories-products.json");
            //var result = ImportCategoryProducts(context, categoriesProductsJson);

            //05
            //var result = GetProductsInRange(context);

            //06
            //var result = GetSoldProducts(context);

            //07
            //var result = GetCategoriesByProductsCount(context);

            var result = GetUsersWithProducts(context);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson)
                .Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.Length >= 3)
                .ToArray();

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Length >= 3)
                .ToArray();

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            Category[] categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
               .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Length >= 3 && x.Name.Length <= 15)
               .ToArray();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.Price)
                .ToList();

            var json = JsonConvert.SerializeObject(productsInRange, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new 
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new 
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                            BuyerFirstName = ps.Buyer.FirstName,
                            BuyerLastName = ps.Buyer.LastName
                        })
                        .ToArray()
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(
                users,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price).ToString("0.00"),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price).ToString("0.00")
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToList();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(ps => ps.Buyer != null))
                .Select(u => new 
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold
                            .Count(ps => ps.Buyer != null),
                        Products = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                        })
                        .ToArray()
                    }
                })
                .ToArray();

            var result = new
            {
                UsersCount = users.Length,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            
            var json = JsonConvert.SerializeObject(
                result,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    }
}