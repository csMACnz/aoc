// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, World!");

Console.WriteLine("Part1: " + new Grid("puzzle.txt").Part1());

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
}