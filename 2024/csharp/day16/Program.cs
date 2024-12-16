// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var grid = new bool[lines.Length, lines[0].Length];
(int row, int col) start = (-1, -1);
(int row, int col) end = (-1, -1);
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
var q = new PriorityQueue<(long score, Facing dir, int x, int y), long>();
q.Enqueue((0, Facing.East, start.row, start.col), 0);
var seen = new HashSet<(int row, int col, Facing)>();
while (q.Count > 0)
{
    var (score, dir, row, col) = q.Dequeue();
    var inWall = grid[row, col];
    if (inWall) continue;
    if (seen.Contains((row, col, dir))) continue;
    seen.Add((row, col, dir));
    if (end.row == row && end.col == col)
    {
        Console.WriteLine("Part1: " + score);
        break;
    }
    if (dir == Facing.East)
    {
        q.Enqueue((score + 1, Facing.East, row, col + 1), score + 1);

    }
    else
    {
        q.Enqueue((score + 1000, Facing.East, row, col), score + 1000);
    }
    if (dir == Facing.West)
    {
        q.Enqueue((score + 1, Facing.West, row, col - 1), score + 1);

    }
    else
    {
        q.Enqueue((score + 1000, Facing.West, row, col), score + 1000);

    }
    if (dir == Facing.North)
    {
        q.Enqueue((score + 1, Facing.North, row - 1, col), score + 1);

    }
    else
    {
        q.Enqueue((score + 1000, Facing.North, row, col), score + 1000);

    }
    if (dir == Facing.South)
    {
        q.Enqueue((score + 1, Facing.South, row + 1, col), score + 1);

    }
    else
    {
        q.Enqueue((score + 1000, Facing.South, row, col), score + 1000);
    }
}

enum Facing
{
    East,
    North,
    West,
    South
}