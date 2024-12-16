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
var q = new PriorityQueue<Node, long>();
q.Enqueue(new Node(null, 0, Facing.East, start.row, start.col), 0);
var best = new List<Node>();
var seen = new Dictionary<(int row, int col, Facing dir), int>();
while (q.Count > 0)
{
    var head = q.Dequeue();
    if (best.Count > 0 && head.Score > best[0].Score) break;
    var inWall = grid[head.Row, head.Col];
    if (inWall) continue;
    if (seen.TryGetValue((head.Row, head.Col, head.Direction), out var score) && score < head.Score)
    {
        continue;
    }
    seen[(head.Row, head.Col, head.Direction)] = head.Score;
    if (end.row == head.Row && end.col == head.Col)
    {
        best.Add(head);
        continue;
    }
    if (head.Direction == Facing.East)
    {
        q.Enqueue(new Node(head, head.Score + 1, Facing.East, head.Row, head.Col + 1), head.Score + 1);
    }
    else
    {
        q.Enqueue(new Node(head, head.Score + 1000, Facing.East, head.Row, head.Col), head.Score + 1000);
    }
    if (head.Direction == Facing.West)
    {
        q.Enqueue(new Node(head, head.Score + 1, Facing.West, head.Row, head.Col - 1), head.Score + 1);

    }
    else
    {
        q.Enqueue(new Node(head, head.Score + 1000, Facing.West, head.Row, head.Col), head.Score + 1000);

    }
    if (head.Direction == Facing.North)
    {
        q.Enqueue(new Node(head, head.Score + 1, Facing.North, head.Row - 1, head.Col), head.Score + 1);

    }
    else
    {
        q.Enqueue(new Node(head, head.Score + 1000, Facing.North, head.Row, head.Col), head.Score + 1000);

    }
    if (head.Direction == Facing.South)
    {
        q.Enqueue(new Node(head, head.Score + 1, Facing.South, head.Row + 1, head.Col), head.Score + 1);

    }
    else
    {
        q.Enqueue(new Node(head, head.Score + 1000, Facing.South, head.Row, head.Col), head.Score + 1000);
    }
}

Console.WriteLine("Part1: " + best[0].Score);
var unique = new HashSet<(int row, int col)>(best.Select(x => (x.Row, x.Col)));
foreach (var h in best)
{
    var head = h;
    while (head != null)
    {
        unique.Add((head.Row, head.Col));
        head = head.Previous;
    }
}
Console.WriteLine("Part2: " + unique.Count);

enum Facing
{
    East,
    North,
    West,
    South
}

record Node(Node? Previous, int Score, Facing Direction, int Row, int Col)
{
}