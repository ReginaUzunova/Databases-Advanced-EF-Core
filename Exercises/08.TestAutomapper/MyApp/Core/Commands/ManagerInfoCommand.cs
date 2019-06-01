using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ManagerInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ManagerInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {

            int employeeId = int.Parse(inputArgs[0]);
            
            var employee = context.Employees
                .Include(e => e.ManagedEmployees)
                .FirstOrDefault(x => x.Id == employeeId);

            if (employee == null)
            {
                throw new ArgumentException("There is not employee with that Id!");
            }

            var managerDto = this.mapper.CreateMappedObject<ManagerDto>(employee);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.ManagedEmployees.Count}");

            foreach (var employeeDto in managerDto.ManagedEmployees)
            {
                sb.AppendLine($"- {employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
