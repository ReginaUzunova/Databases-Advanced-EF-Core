﻿namespace MyApp.Core
{
    using MyApp.Core.Contracts;
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public class Engine : IEngine
    {
        private readonly IServiceProvider provider;

        public Engine(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void Run()
        {
            while (true)
            {
                string[] inputArgs = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

                var commandInterpreter = this.provider.GetService<ICommandInterpreter>();
                string result = commandInterpreter.Read(inputArgs);

                Console.WriteLine(result);
            }
            
        }
    }
}
