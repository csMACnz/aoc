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

Console.WriteLine($"Expected: (50, 24)");
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

        var points = new List<(long x, long y)>();
        var lines = input.EnumerateLines();
        while (lines.MoveNext())
        {
            var line = lines.Current;
            var parts = line.Split(',');
            parts.MoveNext();
            var x = long.Parse(line[parts.Current]);
            parts.MoveNext();
            var y = long.Parse(line[parts.Current]);
            points.Add((x, y));
        }

        var candidates = new SortedSet<(long area, (long x, long y) a, (long x, long y) b)>(
            Comparer<(long area, (long x, long y) a, (long x, long y) b)>.Create(
                (x, y) => -x.area.CompareTo(y.area)));
        for (var i = 0; i < points.Count; i++)
        {
            var a = points[i];
            for (var j = i + 1; j < points.Count; j++)
            {
                var b = points[j];
                var area = (Math.Abs(a.x - b.x) + 1) * (Math.Abs(a.y - b.y) + 1);
                candidates.Add((area, a, b));
            }
        }
        part1 = candidates.First().area;
        foreach(var candidate in candidates)
        {
            var containsAnotherPoint = false;
            for (int i = 0; i < points.Count; i++)
            {
                (long x, long y) point = points[i];
                if (point != candidate.a && point != candidate.b)
                {
                    // if point is inside, not on edge
                    // or if point is on edge but at least one of the lines are not edge aligned
                    // can't work
                    // Or is this shoelace again?
                    // NOT DONE YET
                    
                    var withinX = point.x > Math.Min(candidate.a.x, candidate.b.x) && point.x < Math.Max(candidate.a.x, candidate.b.x);
                    var withinXIncludingEdges = point.x >= Math.Min(candidate.a.x, candidate.b.x) && point.x <= Math.Max(candidate.a.x, candidate.b.x);
                    var withinY = point.y > Math.Min(candidate.a.y, candidate.b.y) && point.y < Math.Max(candidate.a.y, candidate.b.y);
                    var withinYIncludingEdges = point.y >= Math.Min(candidate.a.y, candidate.b.y) && point.y <= Math.Max(candidate.a.y, candidate.b.y);
                    if ((withinX && withinYIncludingEdges) || (withinXIncludingEdges && withinY))
                    {
                        containsAnotherPoint = true;
                        break;
                    }
                }
            }
            if (!containsAnotherPoint)
            {
                part2 = candidate.area;
                break;
            }
        }
        return (part1, part2);
    }
}

public class Tests
{
    [Fact]
    public void TestExample()
    {
        Program.Puzzle(Program.TestInput).Should().Be((50, 24));
    }
}