using System;
using System.IO;
using System.Linq;
using Accounts.Tests;

namespace Accounts.SpecGenerator
{
    class Program
    {
        static void Main()
        {
            var specifications = typeof(Specification<,>).Assembly
                .GetTypes()
                .Where(type =>
                    type.BaseType != null && 
                    type.BaseType.IsGenericType && 
                    type.BaseType.GetGenericTypeDefinition() == typeof(Specification<,>))
                .Select(type => Activator.CreateInstance(type)?.ToString());

            using var sw = File.CreateText("specifications.txt");

            foreach (var specification in specifications)
            {
                sw.WriteLine("-----------------------------");
                sw.Write(specification);
            }
        }
    }
}
