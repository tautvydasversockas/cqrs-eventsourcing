using Accounts.SpecGenerator;
using Accounts.Tests;
using System;
using System.IO;
using System.Linq;

var specifications = typeof(Specification<,,>).Assembly
    .GetTypes()
    .Where(type =>
        !type.IsAbstract &&
        type.IsSubclassOfRawGeneric(typeof(Specification<,,>)))
    .Select(type => Activator.CreateInstance(type)?.ToString());

using var streamWriter = File.CreateText("specifications.txt");

foreach (var specification in specifications)
{
    streamWriter.WriteLine("-----------------------------");
    streamWriter.Write(specification);
}