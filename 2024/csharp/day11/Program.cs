// See https://aka.ms/new-console-template for more information
using System.IO.Pipelines;

Console.WriteLine("Hello, World!");

var input = File.ReadAllText("puzzle.txt").Split().Select(long.Parse).ToArray();

var cache = new Dictionary<(long value, int repeats), long>();

Console.WriteLine("Part1: " + input.Select(n => HowMany(n, 25)).Sum());

Console.WriteLine("Part1: " + input.Select(n => HowMany(n, 75)).Sum());

long HowMany(long value, int repeats)
{
    if (cache.TryGetValue((value, repeats), out var result)) return result;
    result = (repeats, value) switch
    {
        (1, 0) => 1,
        (1, var x) when ($"{x}".Length % 2 == 0) => 2,
        (1, _) => 1,
        (_, 0) => HowMany(1, repeats - 1),
        (_, var x) when ($"{x}".Length % 2 == 0) => HowMany(long.Parse($"{x}"[..($"{x}".Length / 2)]), repeats - 1) + HowMany(long.Parse($"{x}"[($"{x}".Length / 2)..]), repeats - 1),
        _ => HowMany(value * 2024, repeats - 1)
    };
    cache[(value, repeats)] = result;
    return result;
}