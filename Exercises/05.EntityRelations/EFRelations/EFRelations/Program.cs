using EFRelations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace EFRelations
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddLogging(configure => configure.AddConsole())
                .AddDbContext<StudentSystemContext>(options => 
                {
                    options.UseSqlServer("Server=DESKTOP-8Q6AF3G\\SQLEXPRESS;Database=Stud;Integrated Security=True;",
                        s => s.MigrationsAssembly("EFRelations.Infrastructure"))
                        .EnableSensitiveDataLogging();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = serviceProvider.GetService<StudentSystemContext>();

            var students = context.Students.ToList();
        }
    }
}
