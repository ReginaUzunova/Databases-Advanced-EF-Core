namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    //using Z.EntityFramework.Plus;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //01
                //var result = GetBooksByAgeRestriction(db, "miNor");
                //Console.WriteLine(result);

                //02
                //var result = GetGoldenBooks(db);
                //Console.WriteLine(result);

                //03
                //var result = GetBooksByPrice(db);
                //Console.WriteLine(result);

                //04
                //var result = GetBooksNotReleasedIn(db, 2000);
                //Console.WriteLine(result);

                //05
                //var result = GetBooksByCategory(db, "horror mystery drama");
                //Console.WriteLine(result);

                //06
                //var result = GetBooksReleasedBefore(db, "30-12-1989");
                //Console.WriteLine(result);

                //07
                //var result = GetAuthorNamesEndingIn(db, "e");
                //Console.WriteLine(result);

                //08
                //var result = GetBookTitlesContaining(db, "sK");
                //Console.WriteLine(result);

                //09
                //var result = GetBooksByAuthor(db, "R");
                //Console.WriteLine(result);

                //10
                //var result = CountBooks(db, 12);
                //Console.WriteLine(result);

                //11
                //var result = CountCopiesByAuthor(db);
                //Console.WriteLine(result);

                //12
                //var result = GetTotalProfitByCategory(db);
                //Console.WriteLine(result);

                //13
                //var result = GetMostRecentBooks(db);
                //Console.WriteLine(result);

                //14
                //IncreasePrices(db);

                //15
                //DbInitializer.ResetDatabase(db);
                var result = RemoveBooks(db);
                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(t => t.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(e => e.EditionType.ToString() == "Gold" && e.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(t => t.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, goldenBooks);

            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .Where(p => p.Price > 40)
                .OrderByDescending(x => x.Price)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(r => Convert.ToDateTime(r.ReleaseDate).Year != year)
                .OrderBy(b => b.BookId)
                .Select(t => t.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var books = context.Books
                .Where(bc => bc.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(t => t.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(r => r.ReleaseDate.Value < targetDate)
                .OrderByDescending(b => b.ReleaseDate.Value)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
               .Where(n => EF.Functions.Like(n.FirstName, $"%{input}"))
               .Select(a => new
               {
                   a.FirstName,
                   a.LastName,
               })
               .OrderBy(x => x.FirstName)
               .ThenBy(x => x.LastName)
               .ToList();

            var result = string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName}"));

            return result;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
               .Where(b => EF.Functions.Like(b.Title, $"%{input.ToLower()}%"))
               .Select(t => t.Title)
               .OrderBy(x => x)
               .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
               .Where(b => EF.Functions.Like(b.Author.LastName, $"{input.ToLower()}%"))
               .OrderBy(x => x.BookId)
               .Select(x => new
               {
                   x.Title,
                   x.Author.FirstName,
                   x.Author.LastName
               })
               .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} ({x.FirstName} {x.LastName})"));

            return result;
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(t => t.Title)
                .ToList();

            return books.Count;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    BooksCount = x.Books.Sum(c => c.Copies),
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .OrderByDescending(b => b.BooksCount)
                .ToList();

            var result = string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName} - {x.BooksCount}"));

            return result;
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Profit = x.CategoryBooks.Sum(c => c.Book.Copies * c.Book.Price)
                })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.CategoryName)
                .ToList();

            var result = string.Join(Environment.NewLine, categories.Select(x => $"{x.CategoryName} ${x.Profit:F2}"));

            return result;
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .OrderBy(x => x.Name)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    TopBooks = c.CategoryBooks.Select(x => new
                    {
                        x.Book.Title,
                        x.Book.ReleaseDate
                    })
                    .OrderByDescending(x => x.ReleaseDate)
                    .Take(3)
                    .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.TopBooks)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(d => d.ReleaseDate.Value.Year < 2010)
                //.Update(x => new Book() { Price = x.Price + 5})
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }
            

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(c => c.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(books);
            context.SaveChanges();

            return books.Count;
        }
    }
}
