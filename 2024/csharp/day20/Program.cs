// See https://aka.ms/new-console-template for more information
using System.IO.Pipelines;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var rowCount = lines.Length;
var colCount = lines[0].Length;
var grid = new bool[rowCount, colCount];
var start = (-1, -1);
var end = (-1, -1);

foreach (var (row, line) in lines.Index())
{
    foreach (var (col, ch) in line.Index())
    {
        if (ch == '#')
        {
            grid[row, col] = true;
        }
        else if (ch == 'S')
        {
            start = (row, col);
        }
        else if (ch == 'E')
        {
            end = (row, col);
        }
    }
}
Console.WriteLine(start);
Console.WriteLine(end);


var baseCount = CalculateSteps(grid);
Console.WriteLine("Part 0: " + baseCount);
var result = 0;
foreach (var row in Enumerable.Range(0, rowCount))
{
    foreach (var col in Enumerable.Range(0, colCount))
    {
        Console.Write(".");
        if (grid[row, col])
        {
            grid[row, col] = false;
            var ans = CalculateSteps(grid);
            if (baseCount - ans >= 100)
            {
                result++;
            }
            grid[row, col] = true;
        }
    }

    Console.WriteLine();
}


Console.WriteLine("Part 1: " + result);

int CalculateSteps(bool[,] grid)
{
    int rowCount = grid.GetLength(0);
    int colCount = grid.GetLength(1);
    var queue = new Queue<((int row, int col) pos, int count)>();
    queue.Enqueue((start, 0));
    var seen = new bool[rowCount, colCount];
    while (queue.TryDequeue(out var item))
    {
        if (item.pos.row < 0 || item.pos.row >= rowCount || item.pos.col < 0 || item.pos.col >= colCount) continue;
        if (seen[item.pos.row, item.pos.col]) continue;
        seen[item.pos.row, item.pos.col] = true;
        if (item.pos == end)
        {
            return item.count;
        }
        if (grid[item.pos.row, item.pos.col]) continue;
        queue.Enqueue(((item.pos.row + 1, item.pos.col), item.count + 1));
        queue.Enqueue(((item.pos.row - 1, item.pos.col), item.count + 1));
        queue.Enqueue(((item.pos.row, item.pos.col + 1), item.count + 1));
        queue.Enqueue(((item.pos.row, item.pos.col - 1), item.count + 1));
    }
    return int.MinValue;
}