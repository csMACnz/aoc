// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
(int rowOffset, int colOffset, FromDirection nextFromDir)[][] diffs = [
    [(0, 1, FromDirection.Left), (1, 0,FromDirection.Top), (0, -1,FromDirection.Right), (-1, 0,FromDirection.Bottom)],
    [(0, 1, FromDirection.Left), (1, 0,FromDirection.Top), (-1, 0,FromDirection.Bottom)],
    [(0, 1, FromDirection.Left), (0, -1,FromDirection.Right), (-1, 0,FromDirection.Bottom)],
    [(1, 0,FromDirection.Top), (0, -1,FromDirection.Right), (-1, 0,FromDirection.Bottom)],
    [(0, 1, FromDirection.Left), (1, 0,FromDirection.Top), (0, -1,FromDirection.Right)]
    ];

var lines = File.ReadAllLines("puzzle.txt");
var stack = new Stack<(int startRow, int startCol, int row, int col, byte value, FromDirection direction)>();
var grid = new byte[lines.Length, lines[0].Length];
foreach (var (row, line) in lines.Index())
{
    foreach (var (col, ch) in line.Index())
    {
        var digit = byte.Parse(ch.ToString());
        if (digit == 0)
        {
            stack.Push((row, col, row, col, digit, FromDirection.None));
        }
        grid[row, col] = digit;
    }
}

HashSet<(int startRow, int startCol, int endRow, int endCol)> results = [];
while (stack.TryPop(out var current))
{
    if (current.value == 9)
    {
        results.Add((current.startRow, current.startCol, current.row, current.col));
    }
    else
    {
        foreach (var (rowOffset, colOffset, nextFromDir) in diffs[(int)current.direction])
        {
            var check = (row: current.row + rowOffset, col: current.col + colOffset);
            if (check.row >= 0 && check.row < grid.GetLength(0) && check.col >= 0 && check.col < grid.GetLength(1))
            {
                var nextValue = grid[check.row, check.col];
                if (nextValue == (current.value + 1))
                {
                    stack.Push((current.startRow, current.startCol, check.row, check.col, nextValue, nextFromDir));
                }
            }
        }
    }
}
Console.WriteLine($"Part1: {results.Count}");

public enum FromDirection
{
    None = 0,
    Left = 1,
    Bottom = 2,
    Right = 3,
    Top = 4
}