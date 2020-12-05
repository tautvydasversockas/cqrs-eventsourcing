using System;
using System.IO;
using System.Linq;
using Accounts.Tests;

var specifications = typeof(Specification<,>).Assembly
    .GetTypes()
    .Where(type =>
        type.BaseType is not null &&
        type.BaseType.IsGenericType &&
        type.BaseType.GetGenericTypeDefinition() == typeof(Specification<,>))
    .Select(type => Activator.CreateInstance(type)?.ToString());

using var streamWriter = File.CreateText("specifications.txt");

foreach (var specification in specifications)
{
    streamWriter.WriteLine("-----------------------------");
    streamWriter.Write(specification);
}