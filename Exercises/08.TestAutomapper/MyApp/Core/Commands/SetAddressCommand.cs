namespace MyApp.Core.Commands
{
    using MyApp.Core.Commands.Contracts;
    using MyApp.Data;
    using System;
    using System.Linq;

    public class SetAddressCommand : ICommand
    {
        private readonly MyAppContext context;

        public SetAddressCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            string address = inputArgs[1];

            var currentEmployee = context.Employees
                .FirstOrDefault(e => e.Id == employeeId);

            if (currentEmployee == null)
            {
                throw new ArgumentException("There is no employee with that Id!");
            }

            currentEmployee.Address = address;
            this.context.SaveChanges();

            string result = $"Address of {currentEmployee.FirstName} {currentEmployee.LastName} was set successfully!";

            return result;
        }
    }
}
