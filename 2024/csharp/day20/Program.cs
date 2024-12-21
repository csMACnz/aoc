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

var basePath = CalculateSteps(grid, start, end);
var baseCount = basePath.Count - 1;
Console.WriteLine("(9376) Part 0: " + baseCount);

var cheatSize = 2;
var result = 0;
foreach (var index in Enumerable.Range(0, baseCount))
{
    var cheatStartPos = basePath[index];
    foreach (var yx in OffsetsToCheck(cheatSize))
    {
        var cheatEndPos = (row: cheatStartPos.row + yx.row, col: cheatStartPos.col + yx.col);
        if (cheatEndPos.row < 0 || cheatEndPos.row >= rowCount ||
            cheatEndPos.col < 0 || cheatEndPos.col >= colCount) continue;
        if (grid[cheatEndPos.row, cheatEndPos.col]) continue;
        var postCheatSteps = CalculateSteps(grid, cheatEndPos, end);
        var ans = index + cheatSize + postCheatSteps.Count - 1;
        if (baseCount - ans >= 100)
        {
            result++;
        }
    }
}

Console.WriteLine("(1343) Part 1: " + result);
return;

// var result = Part1BruteForce(grid);
// Console.WriteLine("(1343) Part 1: " + result);

int Part1BruteForce(bool[,] grid)
{
    int rowCount = grid.GetLength(0);
    int colCount = grid.GetLength(1);
    var result = 0;
    foreach (var row in Enumerable.Range(0, rowCount))
    {
        foreach (var col in Enumerable.Range(0, colCount))
        {
            Console.Write(".");
            if (grid[row, col])
            {
                grid[row, col] = false;
                var ans = CalculateSteps(grid, start, end).Count;
                if (baseCount - ans >= 100)
                {
                    result++;
                }
                grid[row, col] = true;
            }
        }
    }
    return result;
}

static List<(int row, int col)> CalculateSteps(bool[,] grid, (int, int) start, (int, int) end)
{
    int rowCount = grid.GetLength(0);
    int colCount = grid.GetLength(1);
    var queue = new Queue<Node>();
    queue.Enqueue(new Node(null, start, 0));
    var seen = new bool[rowCount, colCount];
    while (queue.TryDequeue(out var item))
    {
        if (item.pos.row < 0 || item.pos.row >= rowCount || item.pos.col < 0 || item.pos.col >= colCount) continue;
        if (seen[item.pos.row, item.pos.col]) continue;
        seen[item.pos.row, item.pos.col] = true;
        if (item.pos == end)
        {
            var list = new List<(int row, int col)>
            {
                item.pos
            };
            while (item.prev is not null)
            {
                item = item.prev;
                list.Add(item.pos);
            }
            list.Reverse();
            return list;
        }
        if (grid[item.pos.row, item.pos.col]) continue;
        queue.Enqueue(new Node(item, (item.pos.row + 1, item.pos.col), item.count + 1));
        queue.Enqueue(new Node(item, (item.pos.row - 1, item.pos.col), item.count + 1));
        queue.Enqueue(new Node(item, (item.pos.row, item.pos.col + 1), item.count + 1));
        queue.Enqueue(new Node(item, (item.pos.row, item.pos.col - 1), item.count + 1));
    }
    return null!;
}

IEnumerable<(int row, int col)> OffsetsToCheck(int range)
{
    foreach (var i in Enumerable.Range(0, range + 1))
    {
        foreach (var j in Enumerable.Range(0, range + 1 - i))
        {
            if (i != 0 || j != 0)
            {
                yield return (i, j);
                if (i != 0)
                {
                    yield return (-i, j);
                }
                if (j != 0)
                {
                    yield return (i, -j);
                    if (i != 0)
                    {
                        yield return (-i, -j);
                    }
                }
            }
        }
    }
}

record class Node(Node? prev, (int row, int col) pos, int count);
