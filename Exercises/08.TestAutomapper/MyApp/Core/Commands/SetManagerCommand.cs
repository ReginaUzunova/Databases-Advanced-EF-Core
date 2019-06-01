using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetManagerCommand : ICommand
    {
        private readonly MyAppContext context;

        public SetManagerCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            int managerId = int.Parse(inputArgs[1]);

            var employee = context.Employees
                .Find(employeeId);

            var manager = context.Employees
                .Find(managerId);

            if (employee == null || manager == null)
            {
                throw new ArgumentException("There is not employee with that Id!");
            }

            employee.Manager = manager;
            this.context.SaveChanges();

            string result = $"Command completed successfully!";

            return result;
        }
    }
}
