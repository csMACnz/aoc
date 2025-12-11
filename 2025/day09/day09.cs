#!/usr/local/share/dotnet/dotnet run
#:package xunit.v3@3.2.1
#:package Colorful.Console@1.2.15
#:package AwesomeAssertions@9.3.0

using Xunit;
using AwesomeAssertions;
using System.Text.RegularExpressions;
using Xunit.Internal;

if (args.Length > 0 && args[0] == "test")
{
    await Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run([]);
    return;
}

Colorful.Console.WriteAscii("AoC 2025 Day 9");

Console.WriteLine($"Expected: (50, 0)");
Console.WriteLine($" Example: {Puzzle(TestInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/9/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day09: {Puzzle(content)}");

public partial class Program
{
    public static readonly string TestInput = """
    7,1
    11,1
    11,7
    9,7
    9,5
    2,5
    2,3
    7,3
    """;

    public static (long one, long two) Puzzle(ReadOnlySpan<char> input)
    {
        var part1 = 0L;
        var part2 = 0L;

        var points = new List<Point>();
        var lines = input.EnumerateLines();
        while (lines.MoveNext())
        {
            var line = lines.Current;
            var parts = line.Split(',');
            parts.MoveNext();
            var x = long.Parse(line[parts.Current]);
            parts.MoveNext();
            var y = long.Parse(line[parts.Current]);
            points.Add(new Point(x, y));
        }

        var max = 0L;
        for (var i = 0; i < points.Count; i++)
        {
            var a = points[i];
            for (var j = i + 1; j < points.Count; j++)
            {
                var b = points[j];
                var area = (Math.Abs(a.x - b.x) + 1) * (Math.Abs(a.y - b.y) + 1);
                max = Math.Max(max, area);
            }
        }
        part1 = max;
        return (part1, part2);
    }
}

public record struct Point(long x, long y);

public class Tests
{
    [Fact]
    public void TestExample()
    {
        Program.Puzzle(Program.TestInput).Should().Be((50, 0));
    }
}