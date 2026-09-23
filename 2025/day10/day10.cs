#!/usr/local/share/dotnet/dotnet run
#:package xunit.v3@3.2.1
#:package Colorful.Console@1.2.15
#:package AwesomeAssertions@9.3.0

using Xunit;
using AwesomeAssertions;
using System.Text.RegularExpressions;
using Xunit.Internal;
using System.Reflection.PortableExecutable;
using System.Collections.ObjectModel;

if (args.Length > 0 && args[0] == "test")
{
    await Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run([]);
    return;
}

Colorful.Console.WriteAscii("AoC 2025 Day 10");

Console.WriteLine($"Expected: (7, 0)");
Console.WriteLine($" Example: {Puzzle(TestInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/10/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day10: {Puzzle(content)}");
public partial class Program
{
    public static readonly string TestInput = """
    [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
    [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
    [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
    """;

    public static (long one, long two) Puzzle(ReadOnlySpan<char> input)
    {
        var part1 = 0L;
        var part2 = 0L;

        var data  = new List<Machine>();
        foreach (var line in input.EnumerateLines())
        {
            List<bool> target;
            List<List<long>> buttons = [];
            List<long> joltageRequirements = [];
            foreach (var part in line.Split(' '))
            {
                ReadOnlySpan<char> segment = line[part];
                switch (segment[0])
                {
                    case '[':
                        target = [];
                        foreach (var c in segment[1..^1])
                        {
                            target.Add(c == '#');
                        }
                        break;
                    case '(':
                        var schematics = segment[1..^1];
                        var currentButtons = new List<long>();
                        foreach (var c in schematics.Split(','))
                        {
                            currentButtons.Add(long.Parse(schematics[c]));
                        }
                        buttons.Add(currentButtons);
                        break;
                    case '{':
                        joltageRequirements = [];
                        var reqs = segment[1..^1];
                        foreach (var c in reqs.Split(','))
                        {
                            joltageRequirements.Add(long.Parse(reqs[c]));
                        }
                        break;
                }
            }
        }

        //process data

        return (part1, part2);
    }
}

public record Machine(ReadOnlyCollection<bool> Target, ReadOnlyCollection<long> Buttons, ReadOnlyCollection<long> JoltageRequirements);

public class Tests
{
    [Fact]
    public void TestExample()
    {
        Program.Puzzle(Program.TestInput).Should().Be((7, 0));
    }
}