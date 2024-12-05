// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace Day04;

public class ArrayGridSolution
{
    private readonly Letter[][] _data = [];
    private readonly int _row = 0;
    private readonly int _col = 0;

    public ArrayGridSolution(string filename)
    {
        _data = File
            .ReadAllLines(filename)
            .Select(l => l
                .AsEnumerable<char>()
                .Select(character => character switch
                {
                    'X' => Letter.X,
                    'M' => Letter.M,
                    'A' => Letter.A,
                    'S' => Letter.S,
                    char c => throw new UnreachableException($"unexpected char '{c}'")
                }).ToArray())
            .ToArray();
        _row = _data.Length;
        _col = _data[0].Length;
        // Console.WriteLine($"Size: r={row},c={col}");
    }

    public Letter Get(int row, int col)
    {
        if (row >= 0 && row < _row && col >= 0 && col < _col)
        {
            return _data[row][col];
        }
        return Letter.Edge;
    }

    internal int Part1()
    {
        var result = 0;
        (int rowOffset, int colOffset)[] directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];
        foreach (var col in Enumerable.Range(0, _col))
        {
            foreach (var row in Enumerable.Range(0, _row))
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
        }
        return result;
    }

    internal int Part2()
    {
        var result = 0;
        foreach (var col in Enumerable.Range(0, _col))
        {
            foreach (var row in Enumerable.Range(0, _row))
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
        }
        return result;
    }

    internal int Part2RemoveBoundsChecks()
    {
        var result = 0;
        foreach (var col in Enumerable.Range(1, _col - 2))
        {
            foreach (var row in Enumerable.Range(1, _row - 2))
            {
                if (_data[row][col] == Letter.A)
                {
                    if ((((_data[row - 1][col - 1] == Letter.M) && _data[row + 1][col + 1] == Letter.S) || ((_data[row - 1][col - 1] == Letter.S) && _data[row + 1][col + 1] == Letter.M)) &&
                       (((_data[row - 1][col + 1] == Letter.M) && _data[row + 1][col - 1] == Letter.S) || ((_data[row - 1][col + 1] == Letter.S) && _data[row + 1][col - 1] == Letter.M)))
                    {
                        result++;

                    }
                }
            }
        }
        return result;
    }
}
