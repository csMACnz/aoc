#!/usr/local/share/dotnet/dotnet run
#:package xunit.v3@3.2.1
#:package Colorful.Console@1.2.15
#:package AwesomeAssertions@9.3.0

using Xunit;
using AwesomeAssertions;
using Xunit.Internal;

if (args.Length > 0 && args[0] == "test")
{
    await Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run([]);
    return;
}

Colorful.Console.WriteAscii("AoC 2025 Day 7");

Console.WriteLine($"Expected: (21, 0)");
Console.WriteLine($" Example: {Puzzle(TestInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/7/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day07: {Puzzle(content)}");


public partial class Program
{
    public static readonly string TestInput = """
    .......S.......
    ...............
    .......^.......
    ...............
    ......^.^......
    ...............
    .....^.^.^.....
    ...............
    ....^.^...^....
    ...............
    ...^.^...^.^...
    ...............
    ..^...^.....^..
    ...............
    .^.^.^.^.^...^.
    ...............
    """;

    public static (long one, long two) Puzzle(ReadOnlySpan<char> input)
    {
        var part2 = 0L;

        var grid = ParseGrid(input);

        var splitCount = 0;
        var start = grid[0].IndexOf('S');
        var currentBeamIndexes = new HashSet<int> { start };
        for (var row = 1; row < grid.Count; row++)
        {
            var nextBeamIndexes = new HashSet<int>();
            foreach (var beamIndex in currentBeamIndexes)
            {
                var cell = grid[row][beamIndex];
                if (cell == '^')
                {
                    splitCount++;
                    nextBeamIndexes.Add(beamIndex - 1);
                    nextBeamIndexes.Add(beamIndex + 1);
                }
                else
                {
                    nextBeamIndexes.Add(beamIndex);
                }
            }
            currentBeamIndexes = nextBeamIndexes;
            Console.WriteLine($"Row {row}: Beams at {string.Join(", ", currentBeamIndexes)}");
        }

        long part1 = splitCount;

        return (part1, part2);
    }

    public static List<List<char>> ParseGrid(ReadOnlySpan<char> input)
    {

        List<List<char>> grid = [];
        foreach (var line in input.EnumerateLines())
        {
            grid.Add([.. line]);
        }

        return grid;
    }
}

public class Tests
{
    [Fact]
    public void TestExample()
    {
        Program.Puzzle(Program.TestInput).Should().Be((21, 0));
    }

    [Fact]
    public void TestGridParse()
    {
        var grid = Program.ParseGrid(Program.TestInput);

        grid.Count.Should().Be(16);
        foreach (var row in grid)
        {
            row.Count.Should().Be(15);
        }
        grid[0][0].Should().Be('.');
        grid[0][7].Should().Be('S');
        grid[2][7].Should().Be('^');
        grid[14][14].Should().Be('.');
        foreach (var gridCell in grid[^1])
        {
            gridCell.Should().Be('.');
        }
    }
}