// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var grid = new Cell[lines.Length, lines[0].Length];
var instructions = new List<Direction>();
var robotPos = (row: -1, col: -1);
var gridDone = false;
foreach (var (row, line) in lines.Index())
{
    if (line == string.Empty)
    {
        gridDone = true;
    }
    else if (!gridDone)
    {
        foreach (var (col, ch) in line.Index())
        {
            grid[row, col] = ch switch
            {
                '.' => Cell.Empty,
                '#' => Cell.Wall,
                'O' => Cell.Barrel,
                '@' => (robotPos = (row, col), Cell.Robot).Robot,
                _ => throw new UnreachableException($"char:'{ch}'")
            };
        }

    }
    else
    {
        instructions.AddRange(line.Select(ch => ch switch
        {
            '<' => Direction.Left,
            '>' => Direction.Right,
            '^' => Direction.Up,
            'v' => Direction.Down,
            _ => throw new UnreachableException($"char:'{ch}'")
        }));
    }
}

Console.WriteLine(robotPos);

foreach (var direction in instructions)
{
    robotPos = Move(robotPos, grid, direction);
}
var result = Score(grid);
Console.WriteLine($"Part1: {result}");

static (int row, int col) Move((int row, int col) robotPos, Cell[,] grid, Direction direction)
{
    if (grid[robotPos.row, robotPos.col] != Cell.Robot) { throw new UnreachableException("Not Robot"); }
    var (rowOffset, colOffset) = direction switch
    {
        Direction.Up => (-1, 0),
        Direction.Down => (1, 0),
        Direction.Left => (0, -1),
        Direction.Right => (0, 1),
    };
    var rowCount = grid.GetLength(0);
    var colCount = grid.GetLength(1);

    var testRowIndex = robotPos.row + rowOffset;
    var testColIndex = robotPos.col + colOffset;

    var hasSpace = false;
    while (testRowIndex > 0 && testColIndex > 0 && testRowIndex < rowCount && testColIndex < colCount)
    {
        if (grid[testRowIndex, testColIndex] == Cell.Wall)
        {
            break;
        }
        else if (grid[testRowIndex, testColIndex] == Cell.Empty)
        {
            hasSpace = true;
            break;
        }
        else
        {
            if (grid[testRowIndex, testColIndex] != Cell.Barrel)
            {
                throw new UnreachableException("Should be a barrel");
            }

            testRowIndex += rowOffset;
            testColIndex += colOffset;
        }
    }
    if (hasSpace)
    {
        grid[robotPos.row, robotPos.col] = Cell.Empty;
        grid[robotPos.row + rowOffset, robotPos.col + colOffset] = Cell.Robot;
        if (robotPos.row + rowOffset != testRowIndex || robotPos.col + colOffset != testColIndex)
        {
            grid[testRowIndex, testColIndex] = Cell.Barrel;
        }
        return (robotPos.row + rowOffset, robotPos.col + colOffset);
    }
    return robotPos;
}

static int Score(Cell[,] grid)
{
    var result = 0;
    foreach (var row in Enumerable.Range(0, grid.GetLength(0)))
    {
        foreach (var col in Enumerable.Range(0, grid.GetLength(1)))
        {
            if (grid[row, col] == Cell.Barrel)
            {
                result += 100 * row + col;
            }
        }
    }
    return result;
}

public enum Cell
{
    Empty,
    Wall,
    Barrel,
    Robot
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}