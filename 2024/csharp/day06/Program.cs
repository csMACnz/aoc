// See https://aka.ms/new-console-template for more information
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day06>();

Console.WriteLine("Part1: " + new Grid("puzzle.txt").Part1());
Console.WriteLine("Part2 (First): " + new Grid("puzzle.txt").Part2First());
Console.WriteLine("Part2 (Faster): " + new Grid("puzzle.txt").Part2Faster());

[MemoryDiagnoser]
public class Day06
{
    [Benchmark]
    public void Part1()
    {
        new Grid("puzzle.txt").Part1();
    }
    [Benchmark]
    public void Part2First()
    {
        new Grid("puzzle.txt").Part2First();
    }
    [Benchmark]
    public void Part2Faster()
    {
        new Grid("puzzle.txt").Part2Faster();
    }
}

public class Grid
{
    (int rowCount, int colCount) _dimensions;

    private readonly ReadOnlySet<(int, int)> _obsticles;
    private readonly (int rowIndex, int colIndex) _start;

    public Grid(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var obsticles = new HashSet<(int, int)>();
        foreach (var (rowIndex, line) in lines.Index())
        {
            foreach (var (colIndex, c) in line.Index())
            {
                if (c == '#')
                {
                    obsticles.Add((rowIndex, colIndex));
                }
                else if (c == '^')
                {
                    _start = (rowIndex, colIndex);
                }
            }
        }
        _obsticles = new ReadOnlySet<(int, int)>(obsticles);
        _dimensions = (rowCount: lines.Length, colCount: lines[0].Length);
    }

    public int Part1()
    {
        return ResolveExitPath(_dimensions, _start, (-1, 0), _obsticles).Count;
    }

    public int Part2First()
    {
        var count = 0;
        foreach (var rowTest in Enumerable.Range(0, _dimensions.rowCount))
        {
            foreach (var colTest in Enumerable.Range(0, _dimensions.colCount))
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
                while (pos.rowIndex >= 0 && pos.rowIndex < _dimensions.rowCount && pos.colIndex >= 0 && pos.colIndex < _dimensions.colCount)
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

    public int Part2Faster()
    {
        var count = 0;
        foreach (var pathPos in ResolveExitPath(_dimensions, _start, ((int, int))(-1, 0), _obsticles))
        {
            if (_start == pathPos)
            {
                continue;
            }
            var newObsticles = _obsticles.Concat([pathPos]).ToHashSet();
            HashSet<((int, int), (int, int))> visited = [];
            var pos = _start;
            var dir = (-1, 0);
            var stuck = false;
            while (pos.rowIndex >= 0 && pos.rowIndex < _dimensions.rowCount && pos.colIndex >= 0 && pos.colIndex < _dimensions.colCount)
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
        return count;
    }

    private static HashSet<(int, int)> ResolveExitPath((int rowCount, int colCount) dimensions, (int rowIndex, int colIndex) pos, (int rowOffset, int colOffset) dir, ReadOnlySet<(int, int)> obsticles)
    {
        HashSet<(int, int)> visited = [];
        while (pos.rowIndex >= 0 && pos.rowIndex < dimensions.rowCount && pos.colIndex >= 0 && pos.colIndex < dimensions.colCount)
        {
            visited.Add(pos);
            var nextPos = (pos.rowIndex + dir.rowOffset, pos.colIndex + dir.colOffset);
            if (obsticles.Contains(nextPos))
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