#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

using System.Data;
using System.Runtime.CompilerServices;

Colorful.Console.WriteAscii("AoC 2025 Day 4");

var testInput = """
    ..@@.@@@@.
    @@@.@.@.@@
    @@@@@.@.@@
    @.@@@@..@.
    @@.@@@@.@@
    .@@@@@@@.@
    .@.@.@.@@@
    @.@@@.@@@@
    .@@@@@@@@.
    @.@.@@@.@.
    """;

Console.WriteLine($"Expected: (13, 43)");
Console.WriteLine($" Example: {Puzzle(testInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/4/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if(content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
Console.WriteLine($"   Day04: {Puzzle(content)}");

static (long one, long two) Puzzle(ReadOnlySpan<char> input)
{
    var rowOffset = 0;
    var colCount = 0;
    HashSet<(int, int)> rollPositions = [];
    foreach (var line in input.EnumerateLines())
    {
        colCount = line.Length;
        var colOffset = 0;
        foreach (var c in line)
        {
            if (c == '@')
            {
                // Process '@' character
                rollPositions.Add((rowOffset, colOffset));
            }
            colOffset++;
        }
        rowOffset++;
    }
    var rowCount = rowOffset;

    var removeList = GetRemovableRolls(rollPositions, rowCount, colCount);
    var part1 = removeList.Count;
    var part2 = removeList.Count;
    while(removeList.Count > 0)
    {
        foreach (var pos in removeList)
        {
            rollPositions.Remove(pos);
        }
        removeList = GetRemovableRolls(rollPositions, rowCount, colCount);
        part2 += removeList.Count;
    }
    return (part1, part2);
}

static List<(int row, int col)> GetRemovableRolls(HashSet<(int, int)> rollPositions, int rowCount, int colCount)
{
    (int, int)[] offsets = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];
            
    var removeList = new List<(int, int)>();
    for (var row = 0; row < rowCount; row++)
    {
        for (var col = 0; col < colCount; col++)
        {
            if (rollPositions.Contains((row, col)))
            {
                var neighborCount = 0;
                foreach ((int rowOffset, int colOffset) offset in offsets)
                {
                    var nRow = row + offset.rowOffset;
                    var nCol = col + offset.colOffset;
                    if (rollPositions.Contains((nRow, nCol)))
                    {
                        neighborCount++;
                    }
                }
                if (neighborCount < 4)
                {
                    removeList.Add((row, col));
                }
            }
        }
    }   
    return removeList;
}
