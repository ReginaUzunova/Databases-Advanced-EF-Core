namespace MyApp.Core.Commands
{
    using MyApp.Core.Commands.Contracts;
    using MyApp.Data;
    using System;
    using System.Linq;

    public class EmployeeInfoCommand : ICommand
    {
        private readonly MyAppContext context;

        public EmployeeInfoCommand(MyAppContext context)
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
            
            string result = $"ID: {currentEmployee.Id} - {currentEmployee.FirstName} {currentEmployee.LastName} -  ${currentEmployee.Salary:f2}";

            return result;
        }
    }
}
