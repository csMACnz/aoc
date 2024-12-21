// See https://aka.ms/new-console-template for more information
using System.IO.Pipelines;

Console.WriteLine("Hello, World!");

var filename = "puzzle.txt";
var speedLimit = filename == "puzzle.txt" ? 100 : 50;
var lines = File.ReadAllLines(filename);

var rowCount = lines.Length;
var colCount = lines[0].Length;
var grid = new bool[rowCount, colCount];
var cache = new Dictionary<((int row, int col) start, (int row, int col) end), List<(int row, int col)>>();
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

var basePath = CalculateSteps(start, end);
var baseCount = basePath.Count - 1;
Console.WriteLine("(84:9376) Part 0: " + baseCount);

Console.WriteLine("(1:1343) Part 1: " + CheatCalc(2));
Console.WriteLine("(285:982891) Part 2: " + CheatCalc(20));
return;

// var result = Part1BruteForce(grid);
// Console.WriteLine("(1343) Part 1: " + result);

int CheatCalc(int cheatSize)
{
    var result = 0;
    var offsetsToCheck = OffsetsToCheck(cheatSize).ToList();
    foreach (var index in Enumerable.Range(0, baseCount))
    {
        var cheatStartPos = basePath[index];
        foreach (var yx in offsetsToCheck)
        {
            var cheatEndPos = (row: cheatStartPos.row + yx.row, col: cheatStartPos.col + yx.col);
            if (cheatEndPos.row < 0 || cheatEndPos.row >= rowCount ||
                cheatEndPos.col < 0 || cheatEndPos.col >= colCount) continue;
            if (grid[cheatEndPos.row, cheatEndPos.col]) continue;
            var postCheatSteps = CalculateSteps(cheatEndPos, end);
            var ans = index + int.Abs(yx.row) + int.Abs(yx.col) + postCheatSteps.Count -1;
            if (baseCount - ans >= speedLimit)
            {
                checked
                {
                    result++;
                }
            }
        }
    }
    return result;
}

// int Part1BruteForce(bool[,] grid)
// {
//     int rowCount = grid.GetLength(0);
//     int colCount = grid.GetLength(1);
//     var result = 0;
//     foreach (var row in Enumerable.Range(0, rowCount))
//     {
//         foreach (var col in Enumerable.Range(0, colCount))
//         {
//             Console.Write(".");
//             if (grid[row, col])
//             {
//                 grid[row, col] = false;
//                 var ans = CalculateSteps(grid, start, end).Count;
//                 if (baseCount - ans >= 100)
//                 {
//                     result++;
//                 }
//                 grid[row, col] = true;
//             }
//         }
//     }
//     return result;
// }

List<(int row, int col)> CalculateSteps((int, int) start, (int, int) end)
{
    if (cache.TryGetValue((start, end), out var result)) return result;
    int rowCount = grid.GetLength(0);
    int colCount = grid.GetLength(1);
    var queue = new Queue<Node>();
    queue.Enqueue(new Node(null, start));
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
            cache.Add((start, end), list);
            return list;
        }
        if (grid[item.pos.row, item.pos.col]) continue;
        queue.Enqueue(new Node(item, (item.pos.row + 1, item.pos.col)));
        queue.Enqueue(new Node(item, (item.pos.row - 1, item.pos.col)));
        queue.Enqueue(new Node(item, (item.pos.row, item.pos.col + 1)));
        queue.Enqueue(new Node(item, (item.pos.row, item.pos.col - 1)));
    }
    cache.Add((start, end), null!);
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

record class Node(Node? prev, (int row, int col) pos);
