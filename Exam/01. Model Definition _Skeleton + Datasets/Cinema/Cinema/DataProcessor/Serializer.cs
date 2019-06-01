namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(x => x.Rating >= rating && x.Projections.Count > 0 && x.Projections.Any(p => p.Tickets.Count > 0))
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(t => t.Projections.Sum(p => p.Tickets.Sum(pr => pr.Price)))
                .Select(x => new
                {
                    MovieName = x.Title,
                    Rating = x.Rating.ToString("0.00"),
                    TotalIncomes = x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("0.00"),
                    Customers = x.Projections.SelectMany(p => p.Tickets.Select(t => t.Customer))
                    .Select(c => new
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Balance = c.Balance.ToString("0.00")
                    })
                    .OrderByDescending(b => b.Balance)
                    .ThenBy(f => f.FirstName)
                    .ThenBy(l => l.LastName)
                    .ToArray()
                })
                .Take(10)
                .ToArray();

            var jsonResult = JsonConvert.SerializeObject(movies);

            return jsonResult;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customer = context.Customers
                .Where(x => x.Age >= age)
               .Select(x => new ExportCustomerDto
               {
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   SpentMoney = x.Tickets.Sum(t => t.Price).ToString("0.00"),
                   SpentTime = TimeSpan.FromTicks((long)x.Tickets.Select(t => t.Projection).Select(p => p.Movie.Duration.Ticks).Sum()).ToString(@"hh\:mm\:ss")
               })
               .OrderByDescending(x => decimal.Parse(x.SpentMoney))
               .Take(10)
               .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomerDto[]),
               new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            xmlSerializer.Serialize(new StringWriter(sb), customer, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}