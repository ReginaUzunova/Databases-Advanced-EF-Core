namespace MyApp.Core.Commands
{
    using MyApp.Core.Commands.Contracts;
    using MyApp.Data;
    using System;
    using System.Globalization;
    using System.Linq;

    public class SetBirthdayCommand : ICommand
    {
        private readonly MyAppContext context;
        
        public SetBirthdayCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            DateTime date = DateTime.ParseExact(inputArgs[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var currentEmployee = context.Employees
                .FirstOrDefault(e => e.Id == employeeId);

            if (currentEmployee == null)
            {
                throw new ArgumentException("There is no employee with that Id!");
            }

            currentEmployee.Birthday = date;
            this.context.SaveChanges();

            string result = $"Birthday of {currentEmployee.FirstName} {currentEmployee.LastName} was set successfully!";

            return result;
        }
    }
}
