#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

Colorful.Console.WriteAscii("AoC 2025 Day 6");

var testInput = """
    123 328  51 64 
     45 64  387 23 
      6 98  215 314
    *   +   *   +  
    """;

Console.WriteLine($"Expected: (4277556, 0)");
Console.WriteLine($" Example: {Puzzle(testInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/6/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day06: {Puzzle(content)}");

static (long one, long two) Puzzle(ReadOnlySpan<char> input)
{
    List<List<long>> values = [];
    List<char> operations = [];
    foreach (var line in input.EnumerateLines())
    {
        if (line[0] == '*' || line[0] == '+')
        {
            // parse each symbol and push on operations
            foreach (var c in line)
            {
                if (c == ' ') { continue; }
                operations.Add(c);
            }

        }
        else
        {
            var valueLine = new List<long>();
            var candidate = 0L;
            foreach (var c in line)
            {
                if (c == ' ')
                {
                    if (candidate != 0)
                    {
                        valueLine.Add(candidate);
                        candidate = 0L;
                    }
                    continue;
                }
                candidate = candidate * 10 + (c - '0');
            }
            if (candidate != 0)
            {
                valueLine.Add(candidate);
                candidate = 0L;
            }
            values.Add(valueLine);
        }
    }

    var part1 = 0L;

    for (var column = 0; column < operations.Count; column++)
    {
        var op = operations[column];
        var result = values[0][column];
        for (int i = 1; i < values.Count; i++)
        {
            if (op == '*')
            {
                result *= values[i][column];
            }
            else if (op == '+')
            {
                result += values[i][column];
            }
        }
        part1 += result;
    }

    return (part1, 0L);
}
