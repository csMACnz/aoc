// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var grid = new Cell[lines.Length, lines[0].Length];
var instructions = new List<Direction>();
var robotStartPos = (row: -1, col: -1);
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
                '@' => (robotStartPos = (row, col), Cell.Robot).Robot,
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

Console.WriteLine(robotStartPos);

var part2Grid = new Cell[grid.GetLength(0), grid.GetLength(1) * 2];
foreach (var row in Enumerable.Range(0, grid.GetLength(0)))
{
    foreach (var col in Enumerable.Range(0, grid.GetLength(1)))
    {
        var (l, r) = grid[row, col] switch
        {
            Cell.Empty => (Cell.Empty, Cell.Empty),
            Cell.Wall => (Cell.Wall, Cell.Wall),
            Cell.Barrel => (Cell.LeftBarrel, Cell.RightBarrel),
            Cell.Robot => (Cell.Robot, Cell.Empty)
        };
        part2Grid[row, col * 2] = l; ;
        part2Grid[row, (col * 2) + 1] = r;
    }
}

var robotPos = (robotStartPos.row, robotStartPos.col);
foreach (var direction in instructions)
{
    robotPos = Move(robotPos, grid, direction);
}
var result = Score(grid);
Console.WriteLine($"Part1: {result}");

robotPos = (robotStartPos.row, robotStartPos.col * 2);
foreach (var direction in instructions)
{
    robotPos = MovePart2(robotPos, part2Grid, direction);
}
var part2Result = Score(part2Grid);
Console.WriteLine($"Part2: {part2Result}");


static (int row, int col) MovePart2((int row, int col) robotPos, Cell[,] grid, Direction direction)
{
    if (grid[robotPos.row, robotPos.col] != Cell.Robot) { throw new UnreachableException("Not Robot"); }
    (int rowOffset, int colOffest) offset = direction switch
    {
        Direction.Up => (-1, 0),
        Direction.Down => (1, 0),
        Direction.Left => (0, -1),
        Direction.Right => (0, 1),
    };
    if (part2Recurse(robotPos, grid, offset, true))
    {
        part2Recurse(robotPos, grid, offset, false);
        return (robotPos.row + offset.rowOffset, robotPos.col + offset.colOffest);
    }
    else
    {
        return robotPos;
    }

}

static bool part2Recurse((int row, int col) pos, Cell[,] grid, (int rowOffset, int colOffset) offset, bool trial)
{
    if (!trial && grid[pos.row, pos.col] == Cell.Wall) throw new UnreachableException("tried to move a wall");
    if (grid[pos.row, pos.col] == Cell.Wall) return false;
    if (grid[pos.row, pos.col] == Cell.Empty) return true;
    if (grid[pos.row, pos.col] == Cell.Robot)
    {
        if (part2Recurse((pos.row + offset.rowOffset, pos.col + offset.colOffset), grid, offset, true))
        {
            if (!trial)
            {
                part2Recurse((pos.row + offset.rowOffset, pos.col + offset.colOffset), grid, offset, false);
                grid[pos.row + offset.rowOffset, pos.col + offset.colOffset] = grid[pos.row, pos.col];
                grid[pos.row, pos.col] = Cell.Empty;
            }
            return true;
        }
    }
    if (grid[pos.row, pos.col] is Cell.LeftBarrel or Cell.RightBarrel)
    {
        if (offset.rowOffset == 0)
        {
            //horizontal
            if (part2Recurse((pos.row + offset.rowOffset, pos.col + offset.colOffset), grid, offset, true))
            {
                if (!trial)
                {
                    part2Recurse((pos.row + offset.rowOffset, pos.col + offset.colOffset), grid, offset, false);
                    grid[pos.row + offset.rowOffset, pos.col + offset.colOffset] = grid[pos.row, pos.col];
                    grid[pos.row, pos.col] = Cell.Empty;
                }
                return true;
            }
        }
        else
        {
            //vertical
            var horizontalOffset = grid[pos.row, pos.col] == Cell.LeftBarrel ? +1 : -1;
            if (part2Recurse((pos.row + offset.rowOffset, pos.col), grid, offset, true) && part2Recurse((pos.row + offset.rowOffset, pos.col + horizontalOffset), grid, offset, true))
            {
                if (!trial)
                {
                    part2Recurse((pos.row + offset.rowOffset, pos.col + offset.colOffset), grid, offset, false);
                    part2Recurse((pos.row + offset.rowOffset, pos.col + horizontalOffset), grid, offset, false);
                    grid[pos.row + offset.rowOffset, pos.col] = grid[pos.row, pos.col];
                    grid[pos.row + offset.rowOffset, pos.col + horizontalOffset] = grid[pos.row, pos.col + horizontalOffset];
                    grid[pos.row, pos.col] = Cell.Empty;
                    grid[pos.row, pos.col + horizontalOffset] = Cell.Empty;
                }
                return true;
            }
        }
    }
    if (!trial) throw new UnreachableException("tried to move when not moveable");
    return false;
}

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
            if (grid[row, col] is Cell.Barrel or Cell.LeftBarrel)
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
    LeftBarrel,
    RightBarrel,
    Robot
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}