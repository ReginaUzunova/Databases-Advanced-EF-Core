using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });

            var userXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\ProductShop - Skeleton\ProductShop\Datasets\users.xml");
            var productsXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\ProductShop - Skeleton\ProductShop\Datasets\products.xml");
            var categoryXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\ProductShop - Skeleton\ProductShop\Datasets\categories.xml");
            var categoryProductsXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\ProductShop - Skeleton\ProductShop\Datasets\categories-products.xml");

            using (ProductShopContext context = new ProductShopContext())
            {
                //01
                //var users = ImportUsers(context, userXml);

                //02
                //var products = ImportProducts(context, productsXml);

                //03
                //var categories = ImportCategories(context, categoryXml);

                //04
                //var categories = ImportCategoryProducts(context, categoryProductsXml);

                //05
                //var result = GetProductsInRange(context);
                //Console.WriteLine(result);

                //06
                //var result = GetSoldProducts(context);
                //Console.WriteLine(result);

                //07
                //var result = GetCategoriesByProductsCount(context);
                //Console.WriteLine(result);


                var result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]),
                new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]),
                new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var productDto in productsDto)
            {
                var product = Mapper.Map<Product>(productDto);
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]),
                new XmlRootAttribute("Categories"));

            var categoriesDto = (ImportCategoryDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach (var categoryDto in categoriesDto)
            {
                if (categoryDto.Name == null)
                {
                    continue;
                }

                var category = Mapper.Map<Category>(categoryDto);
                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDto[]),
                new XmlRootAttribute("CategoryProducts"));

            var categoriesProductsDto = (ImportCategoryProductsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoriesProducts = new List<CategoryProduct>();

            foreach (var categoryProductsDto in categoriesProductsDto)
            {
                var product = context.Products.Find(categoryProductsDto.ProductId);
                var category = context.Categories.Find(categoryProductsDto.CategoryId);

                if (product == null || category == null)
                {
                    continue;
                }

                var categoryProduct = new CategoryProduct
                {
                    ProductId = product.Id,
                    CategoryId = category.Id
                };

                categoriesProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new ExportProductInRangeDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductInRangeDto[]),
                new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count() > 0)
                .Select(x => new ExportUserDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProductDto = x.ProductsSold.Select(y => new ProductDto
                    {
                        Name = y.Name,
                        Price = y.Price
                    })
                    .ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]),
                new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
               .Select(x => new ExportCategoryDto
               {
                   Name = x.Name,
                   Count = x.CategoryProducts.Count,
                   AveragePrice = x.CategoryProducts.Average(p => p.Product.Price),
                   TotalRevenue = x.CategoryProducts.Sum(p => p.Product.Price)
               })
               .OrderByDescending(x => x.Count)
               .ThenBy(x => x.TotalRevenue)
               .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCategoryDto[]),
                new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
               .Where(u => u.ProductsSold.Count() > 0)
               .OrderByDescending(x => x.ProductsSold.Count)
               .Select(x => new ExportUserAndProductDto
               {
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Age = x.Age,
                   SoldProductDto = new SoldProductDto
                   {
                       Count = x.ProductsSold.Count,
                       ProductDtos = x.ProductsSold.Select(y => new ProductDto
                       {
                           Name = y.Name,
                           Price = y.Price
                       })
                       .OrderByDescending(p => p.Price)
                       .ToArray()
                   }
               })
               .Take(10)
               .ToArray();

            var customExport = new ExportCustomDto
            {
                Count = context.Users
               .Count(u => u.ProductsSold.Count() > 0),
                ExportUserAndProductDto = users
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomDto),
                new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), customExport, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}