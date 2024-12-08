// See https://aka.ms/new-console-template for more information
using System.Collections.ObjectModel;
using Spectre.Console;

public class ArrayGrid
{
    private readonly bool[][] _grid;
    private readonly (int rowIndex, int colIndex) _start;
    private readonly (int rowCount, int colCount) _dimensions;

    public ArrayGrid(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var start = (-1, -1);
        _grid = lines.Select((l, r) => l.Select((ch, c) => { if (ch == '^') { start = (r, c); } return ch == '#'; }).ToArray()).ToArray();
        _start = start;
        _dimensions = (_grid.Length, _grid[0].Length);
    }

    public int Part1()
    {
        return ResolveExitPath(_dimensions, _start, (-1, 0), _grid).Count;
    }

    public int Part2First()
    {
        var count = 0;
        foreach (var rowTest in Enumerable.Range(0, _dimensions.rowCount))
        {
            foreach (var colTest in Enumerable.Range(0, _dimensions.colCount))
            {
                if (_start == (rowTest, colTest) || _grid[rowTest][colTest])
                {
                    continue;
                }
                HashSet<((int, int), (int, int))> visited = [];
                var pos = _start;
                var dir = (-1, 0);
                var stuck = false;
                while (true)
                {
                    if (visited.Contains((pos, dir)))
                    {
                        stuck = true;
                        break;
                    }
                    visited.Add((pos, dir));
                    var nextPos = (rowIndex: pos.rowIndex + dir.Item1, colIndex: pos.colIndex + dir.Item2);
                    if (nextPos.rowIndex < 0 || nextPos.rowIndex >= _dimensions.rowCount || nextPos.colIndex < 0 || nextPos.colIndex >= _dimensions.colCount)
                    {
                        break;
                    }
                    if (_grid[nextPos.rowIndex][nextPos.colIndex] || nextPos == (rowTest, colTest))
                    {
                        dir = (dir.Item2, -dir.Item1);
                    }
                    else
                    {
                        pos = nextPos;
                    }
                }
                if (stuck)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public int Part2Faster()
    {
        var count = 0;
        foreach (var pathPos in ResolveExitPath(_dimensions, _start, ((int, int))(-1, 0), _grid))
        {
            if (_start == pathPos)
            {
                continue;
            }
            HashSet<((int, int), (int, int))> visited = [];
            var pos = _start;
            var dir = (-1, 0);
            var stuck = false;
            while (true)
            {
                if (visited.Contains((pos, dir)))
                {
                    stuck = true;
                    break;
                }
                visited.Add((pos, dir));
                var nextPos = (rowIndex: pos.rowIndex + dir.Item1, colIndex: pos.colIndex + dir.Item2);
                if (nextPos.rowIndex < 0 || nextPos.rowIndex >= _dimensions.rowCount || nextPos.colIndex < 0 || nextPos.colIndex >= _dimensions.colCount)
                {
                    break;
                }
                if (_grid[nextPos.rowIndex][nextPos.colIndex] || nextPos == pathPos)
                {
                    dir = (dir.Item2, -dir.Item1);
                }
                else
                {
                    pos = nextPos;
                }
            }
            if (stuck)
            {
                count++;
            }

        }
        return count;
    }

    private static HashSet<(int, int)> ResolveExitPath((int rowCount, int colCount) dimensions, (int rowIndex, int colIndex) pos, (int rowOffset, int colOffset) dir, bool[][] grid)
    {
        HashSet<(int, int)> visited = [];
        while (true)
        {
            visited.Add(pos);
            var nextPos = (rowIndex: pos.rowIndex + dir.rowOffset, colIndex: pos.colIndex + dir.colOffset);
            if (nextPos.rowIndex < 0 || nextPos.rowIndex >= dimensions.rowCount || nextPos.colIndex < 0 || nextPos.colIndex >= dimensions.colCount)
            {
                break;
            }
            if (grid[nextPos.rowIndex][nextPos.colIndex])
            {
                dir = (dir.colOffset, -dir.rowOffset);
            }
            else
            {
                pos = nextPos;
            }
        }
        return visited;
    }
}