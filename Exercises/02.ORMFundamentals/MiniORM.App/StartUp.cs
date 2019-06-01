namespace MiniORM.App
{
    using Data;
    using Data.Entities;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var connectionString = @"Server=DESKTOP-8Q6AF3G\SQLEXPRESS;Database=MiniORM;Integrated Security=True";

            var context = new SoftUniDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true,
            });

            var employee = context.Employees.Last();
            employee.FirstName = "Modified";

            context.Departments.Add(new Department
            {
                Name = "Sales"
            });

            context.Departments.Remove(context.Departments.Where(x => x.Name == "Sales").First());


            context.SaveChanges();
        }
    }
}
