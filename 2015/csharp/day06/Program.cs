// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var part1Grid = new bool[1000, 1000];
var part2Grid = new int[1000, 1000];
foreach (var line in lines)
{
    int prefixLength;
    Func<bool, bool> part1Action;
    Func<int, int> part2Action;
    //toggle x1,y1 through x2,y2
    if (line.StartsWith("toggle "))
    {
        // Toggle
        prefixLength = 7;
        part1Action = x => !x;
        part2Action = x => x + 2;
    }
    //turn on x1,y1 through x2,y2
    else if (line.StartsWith("turn on "))
    {
        // ON
        prefixLength = 8;
        part1Action = x => true;
        part2Action = x => x + 1;
    }
    //turn off x1,y1 through x2,y2
    else if (line.StartsWith("turn off "))
    {
        // OFF
        prefixLength = 9;
        part1Action = x => false;
        part2Action = x => Math.Max(0, x - 1);
    }
    else
    {
        throw new UnreachableException();
    }

    var (from, to) = ParseRange(line.AsSpan()[prefixLength..]);
    SetRange(part1Grid, from, to, part1Action);
    SetRange(part2Grid, from, to, part2Action);
}

var Part1OnCount = part1Grid.Cast<bool>().Count(b => b);
var Part2totalBrightness = part2Grid.Cast<int>().Sum();

Console.WriteLine("Part1: " + Part1OnCount);
Console.WriteLine("Part2: " + Part2totalBrightness);

static void SetRange<T>(T[,] grid, (int x, int y) from, (int x, int y) to, Func<T, T> desiredState)
{
    foreach (var x in Enumerable.Range(from.x, to.x - from.x + 1))
    {
        foreach (var y in Enumerable.Range(from.y, to.y - from.y + 1))
        {
            grid[x, y] = desiredState(grid[x, y]);
        }
    }
}

static ((int x, int y) from, (int x, int y) to) ParseRange(ReadOnlySpan<char> line)
{
    var partRanges = line.Split(' ');
    partRanges.MoveNext();
    var pair = ParsePair(line[partRanges.Current]);
    partRanges.MoveNext();
    partRanges.MoveNext();
    var pair2 = ParsePair(line[partRanges.Current]);
    return (pair, pair2);
}

static (int x, int y) ParsePair(ReadOnlySpan<char> csv)
{
    var partRanges = csv.Split(',');
    partRanges.MoveNext();
    var x = int.Parse(csv[partRanges.Current]);
    partRanges.MoveNext();
    var y = int.Parse(csv[partRanges.Current]);
    return (x, y);
}