// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

namespace Day04;

public class DictionaryGridWithXACacheSolution
{
    private readonly Dictionary<(int, int), Letter> _data = [];
    private readonly HashSet<(int, int)> _aCache = [];
    private readonly HashSet<(int, int)> _xCache = [];
    private readonly int _row = 0;
    private readonly int _col = 0;

    public DictionaryGridWithXACacheSolution(string filename)
    {
        using StreamReader r = new(filename);
        char[] buffer = new char[1024];
        int read;
        while ((read = r.ReadBlock(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                var position = (_row, _col);
                _data[position] = buffer[i] switch
                {
                    'X' => _xCache.Add(position) ? Letter.X : Letter.X,
                    'M' => Letter.M,
                    'A' => _aCache.Add(position) ? Letter.A : Letter.A,
                    'S' => Letter.S,
                    '\n' => (Letter.Edge, _col = -1, _row++).Edge,
                    '\r' => Letter.Edge,
                    char c => throw new UnreachableException($"char '{c}'")
                };
                _col++;
            }
        }
        _row++;
        // Console.WriteLine($"Size: r={row},c={col}");
    }

    public Letter Get(int row, int col)
    {
        return _data.TryGetValue((row, col), out var match) ? match : Letter.Edge;
    }

    internal int Part1()
    {
        var result = 0;
        (int rowOffset, int colOffset)[] directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];
        foreach (var (row, col) in _xCache)
        {
            if (Get(row, col) == Letter.X)
            {
                foreach (var (rowOffset, colOffset) in directions)
                {
                    if (Get(row + rowOffset, col + colOffset) == Letter.M &&
                    Get(row + (rowOffset * 2), col + (colOffset * 2)) == Letter.A &&
                    Get(row + (rowOffset * 3), col + (colOffset * 3)) == Letter.S)
                    {
                        result++;
                    }
                }
            }

        }
        return result;
    }



    internal int Part2()
    {
        var result = 0;
        foreach (var (row, col) in _aCache)
        {
            if (Get(row, col) == Letter.A)
            {
                if ((((Get(row - 1, col - 1) == Letter.M) && Get(row + 1, col + 1) == Letter.S) || ((Get(row - 1, col - 1) == Letter.S) && Get(row + 1, col + 1) == Letter.M)) &&
                   (((Get(row - 1, col + 1) == Letter.M) && Get(row + 1, col - 1) == Letter.S) || ((Get(row - 1, col + 1) == Letter.S) && Get(row + 1, col - 1) == Letter.M)))
                {
                    result++;

                }

            }
        }
        return result;
    }
}
