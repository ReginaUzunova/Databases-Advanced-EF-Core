namespace MyApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class MyAppContext : DbContext
    {
        public MyAppContext()
        {

        }

        public MyAppContext(DbContextOptions<MyAppContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-8Q6AF3G\\SQLEXPRESS;Database=MySpecialApp;Integrated Security=True;",
                s => s.MigrationsAssembly("MyApp"));

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
