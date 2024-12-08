// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, World!");

Console.WriteLine("Part1: " + new Grid("puzzle.txt").Part1());
Console.WriteLine("Part2: " + new Grid("puzzle.txt").Part2());

public class Grid
{
    private readonly int _rowCount;
    private readonly int _colCount;

    private readonly HashSet<(int, int)> _obsticles = [];
    private readonly (int rowIndex, int colIndex) _start;

    public Grid(string filename)
    {
        var lines = File.ReadAllLines(filename);
        foreach (var (rowIndex, line) in lines.Index())
        {
            foreach (var (colIndex, c) in line.Index())
            {
                if (c == '#')
                {
                    _obsticles.Add((rowIndex, colIndex));
                }
                else if (c == '^')
                {
                    _start = (rowIndex, colIndex);
                }
            }
        }

        _rowCount = lines.Length;
        _colCount = lines[0].Length;
    }

    public int Part1()
    {
        HashSet<(int, int)> visited = new();
        var pos = _start;
        var dir = (-1, 0);
        while (pos.rowIndex >= 0 && pos.rowIndex < _rowCount && pos.colIndex >= 0 && pos.colIndex < _colCount)
        {
            visited.Add(pos);
            var nextPos = (pos.rowIndex + dir.Item1, pos.colIndex + dir.Item2);
            if (_obsticles.Contains(nextPos))
            {
                dir = (dir.Item2, -dir.Item1);
                nextPos = (pos.rowIndex + dir.Item1, pos.colIndex + dir.Item2);
            }
            pos = nextPos;
        }
        return visited.Count;
    }

    public int Part2()
    {
        var count = 0;
        foreach (var rowTest in Enumerable.Range(0, _rowCount))
        {
            foreach (var colTest in Enumerable.Range(0, _colCount))
            {
                if (_start == (rowTest, colTest) || _obsticles.Contains((rowTest, colTest)))
                {
                    continue;
                }
                var newObsticles = _obsticles.Concat([(rowTest, colTest)]).ToHashSet();
                HashSet<((int, int), (int, int))> visited = [];
                var pos = _start;
                var dir = (-1, 0);
                var stuck = false;
                while (pos.rowIndex >= 0 && pos.rowIndex < _rowCount && pos.colIndex >= 0 && pos.colIndex < _colCount)
                {
                    if (visited.Contains((pos, dir)))
                    {
                        stuck = true;
                        break;
                    }
                    visited.Add((pos, dir));
                    var nextPos = (pos.rowIndex + dir.Item1, pos.colIndex + dir.Item2);
                    if (newObsticles.Contains(nextPos))
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
}