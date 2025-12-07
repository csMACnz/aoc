#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

using System.Data;

Colorful.Console.WriteAscii("AoC 2025 Day 5");

var testInput = """
    3-5
    10-14
    16-20
    12-18

    1
    5
    8
    11
    17
    32
    """;

Console.WriteLine($"Expected: (3, 0)");
Console.WriteLine($" Example: {Puzzle(testInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/5/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
if (content.Contains("Please log in"))
{
    throw new Exception("Failed to fetch input, are you logged in?");
}
// Console.WriteLine(content);
Console.WriteLine($"   Day05: {Puzzle(content)}");

static (long one, long two) Puzzle(ReadOnlySpan<char> input)
{
    var part1 = 0L;
    var part2 = 0L;
    var lines = input.EnumerateLines();
    var ranges = new SortedSet<Range>();
    var itemIds = new HashSet<long>();
    while (lines.MoveNext())
    {
        if (lines.Current.Length == 0)
        {
            break;
        }
        var line = lines.Current;
        var parts = line.Split('-');
        parts.MoveNext();
        var start = long.Parse(line[parts.Current]);
        parts.MoveNext();
        var end = long.Parse(line[parts.Current]);
        ranges.Add(new Range(start, end));
    }

    while (lines.MoveNext())
    {
        var id = long.Parse(lines.Current);
        itemIds.Add(id);
    }

    Merge(ranges);

    foreach (var id in itemIds)
    {
        foreach (var range in ranges)
        {
            if (id < range.Start)
            {
                // Not in this range or any future ranges
                break;
            }
            if (id >= range.Start && id <= range.End)
            {
                part1++;
                break;
            }
        }
    }

    foreach(var range in ranges)
    {
        part2 += range.End - range.Start + 1;
    }

    return (part1, part2);
}

static void Merge(SortedSet<Range> ranges)
{
    var index = 0;
    while(index < ranges.Count - 1)
    {
        var current = ranges.ElementAt(index);
        var next = ranges.ElementAt(index + 1);
        if (next.Start <= current.End)
        {
            // Overlap
            ranges.Remove(current);
            ranges.Remove(next);
            var newRange = new Range(current.Start, Math.Max(current.End, next.End));
            ranges.Add(newRange);
        }
        else
        {
            index++;
        }
    }
}

readonly record struct Range(long Start, long End) : IComparable<Range>
{
    public readonly int CompareTo(Range other)
    {
        int startComparison = Start.CompareTo(other.Start);
        if (startComparison != 0)
        {
            return startComparison;
        }
        return End.CompareTo(other.End);
    }
}