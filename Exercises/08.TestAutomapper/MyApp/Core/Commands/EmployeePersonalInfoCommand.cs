namespace MyApp.Core.Commands
{
    using MyApp.Core.Commands.Contracts;
    using MyApp.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;

        public EmployeePersonalInfoCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);

            var currentEmployee = context.Employees
                .FirstOrDefault(e => e.Id == employeeId);

            if (currentEmployee == null)
            {
                throw new ArgumentException("There is no employee with that Id!");
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"ID: {currentEmployee.Id} - {currentEmployee.FirstName} {currentEmployee.LastName} - ${currentEmployee.Salary:f2}");
            sb.AppendLine($"Birthday: {currentEmployee.Birthday.Value.ToString("dd-MM-yyyy")}");
            sb.AppendLine($"Address: {currentEmployee.Address}");


            return sb.ToString().TrimEnd();
        }
    }
}
