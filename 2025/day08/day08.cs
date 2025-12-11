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

Colorful.Console.WriteAscii("AoC 2025 Day 8");

Console.WriteLine($"Expected: (40, 25272)");
Console.WriteLine($" Example: {Puzzle(TestInput, 10)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/8/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day08: {Puzzle(content, 1000)}");

public partial class Program
{
    public static readonly string TestInput = """
    162,817,812
    57,618,57
    906,360,560
    592,479,940
    352,342,300
    466,668,158
    542,29,236
    431,825,988
    739,650,466
    52,470,668
    216,146,977
    819,987,18
    117,168,530
    805,96,715
    346,949,466
    970,615,88
    941,993,340
    862,61,35
    984,92,344
    425,690,689
    """;

    public static (long one, long two) Puzzle(ReadOnlySpan<char> input, int joinCount)
    {
        var part1 = 0L;
        var part2 = 0L;

        var points = new List<(long x, long y, long z)>();
        foreach (var row in input.EnumerateLines())
        {
            var parts = row.Split(',');
            parts.MoveNext();
            var x = long.Parse(row[parts.Current]);
            parts.MoveNext();
            var y = long.Parse(row[parts.Current]);
            parts.MoveNext();
            var z = long.Parse(row[parts.Current]);

            points.Add((x, y, z));
        }
        var disconnected = new HashSet<(long x, long y, long z)>(points);
        var pairs = new SortedSet<(double distance, (long x, long y, long z) a, (long x, long y, long z) b)>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var a = points[i];
                var b = points[j];
                double distance = Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));

                pairs.Add((distance, a, b));
            }
        }

        List<HashSet<(long x, long y, long z)>> groups = [];
        for (var index = 0; index < joinCount; index++)
        {
            var (_, a, b) = pairs.ElementAt(index);
            Connect(disconnected, groups, a, b);
        }

        part1 = groups.OrderByDescending(g => g.Count).Take(3).Aggregate(1L, (a, b) => a * b.Count);

        var nextIndex = joinCount;
        while (disconnected.Count > 0)
        {
            var (_, a, b) = pairs.ElementAt(nextIndex);
            Connect(disconnected, groups, a, b);
            // Console.WriteLine($"{a} <==> {b}");
            part2 = a.x * b.x;
            
            nextIndex++;
        }

        return (part1, part2);
    }

    private static void Connect(HashSet<(long x, long y, long z)> disconnected, List<HashSet<(long x, long y, long z)>> groups, (long x, long y, long z) a, (long x, long y, long z) b)
    {
        List<HashSet<(long x, long y, long z)>> groupsToRemove = [];
        HashSet<(long x, long y, long z)> newGroup = [a, b];
        foreach (var group in groups)
        {
            if (group.Contains(a) || group.Contains(b))
            {
                groupsToRemove.Add(group);
                newGroup.AddRange(group);
            }
            if(group.Contains(a) && group.Contains(b))
            {
                // break early, already connected
                break;
            }
        }
        groups.Add(newGroup);
        foreach (var group in groupsToRemove)
        {
            groups.Remove(group);
        }
        disconnected.Remove(a);
        disconnected.Remove(b);
    }
}

public class Tests
{
    [Fact]
    public void TestExample_A()
    {
        Program.Puzzle(Program.TestInput, 3).Should().Be((6, 25272));
    }

    [Fact]
    public void TestExample_B()
    {
        Program.Puzzle(Program.TestInput, 10).Should().Be((40, 25272));
    }
}