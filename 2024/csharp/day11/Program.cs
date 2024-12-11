// See https://aka.ms/new-console-template for more information
using System.IO.Pipelines;

Console.WriteLine("Hello, World!");

var input = File.ReadAllText("puzzle.txt").Split().Select(long.Parse).ToArray();

foreach (var _ in Enumerable.Repeat(0, 25))
{
    var result = new List<long>();
    foreach (var num in input)
    {
        if (num == 0)
        {
            result.Add(1);
        }
        else if ($"{num}".Length % 2 == 0)
        {
            var str = $"{num}";
            result.Add(long.Parse(str[..(str.Length / 2)]));
            result.Add(long.Parse(str[(str.Length / 2)..]));
        }
        else
        {
            result.Add(num * 2024);
        }
    }
    input = [.. result];
}

Console.WriteLine(input.Length);