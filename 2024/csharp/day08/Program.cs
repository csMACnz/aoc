// See https://aka.ms/new-console-template for more information
using Dumpify;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var data = new List<(int row, int col)>[26 + 26 + 10];
foreach (var i in Enumerable.Range(0, data.Length))
{
    data[i] = [];
}

foreach (var (rowIndex, line) in lines.Index())
{
    foreach ((var colIndex, char ch) in line.Index())
    {
        var insertIndex = ch switch
        {
            _ when ch is >= 'a' and <= 'z' => ch - 'a',
            _ when ch is >= 'A' and <= 'Z' => 26 + ch - 'A',
            _ when ch is >= '0' and <= '9' => 52 + ch - '0',
            _ => -1,
        };
        if (insertIndex >= 0)
        {
            data[insertIndex].Add((rowIndex, colIndex));
        }
    }
}

var part1Grid = new int[lines.Length, lines[0].Length];
var part2Grid = new int[lines.Length, lines[0].Length];

foreach (var list in data)
{
    foreach (var (rowIndex, point1) in list.Index())
    {
        part2Grid[point1.row, point1.col] = 1;
        foreach (var point2 in list.Skip(rowIndex + 1))
        {
            part2Grid[point2.row, point2.col] = 1;
            var rDiff = point1.row - point2.row;
            var cDiff = point1.col - point2.col;
            var test = (row: point1.row + rDiff, col: point1.col + cDiff);
            var part1Stored = false;
            while (test.row >= 0 && test.row < lines.Length && test.col >= 0 && test.col < lines[0].Length)
            {
                if (!part1Stored)
                {
                    part1Stored = true;
                    part1Grid[test.row, test.col] = 1;
                }
                part2Grid[test.row, test.col] = 1;
                test = (row: test.row + rDiff, col: test.col + cDiff);
            }
            part1Stored = false;
            test = (row: point2.row - rDiff, col: point2.col - cDiff);
            while (test.row >= 0 && test.row < lines.Length && test.col >= 0 && test.col < lines[0].Length)
            {
                if (!part1Stored)
                {
                    part1Stored = true;
                    part1Grid[test.row, test.col] = 1;
                }
                part2Grid[test.row, test.col] = 1;
                test = (row: test.row - rDiff, col: test.col - cDiff);
            }
        }
    }
}

// resultsGrid.Dump();
var part1Result = (
        from int val
        in part1Grid
        select val)
    .Sum();

var part2Result = (
        from int val
        in part2Grid
        select val)
    .Sum();


Console.WriteLine("Part1: " + part1Result);
Console.WriteLine("Part2: " + part2Result);