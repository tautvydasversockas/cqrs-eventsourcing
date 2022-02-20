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