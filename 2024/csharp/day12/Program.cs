// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadLines("puzzle.txt");

var data = Enumerable.Repeat(() => new HashSet<(int row, int col)>(), 26).Select(x => x()).ToArray();

foreach (var (row, line) in lines.Index())
{
    foreach (var (col, ch) in line.Index())
    {
        data[ch - 'A'].Add((row, col));
    }
}

var part1Result = 0;
var part2Result = 0;
foreach (var (ch, set) in data.Index())
{
    while (set.Count > 0)
    {
        var newSet = new HashSet<(int row, int col)>();
        var first = set.First();
        set.Remove(first);
        var stack = new Stack<(int row, int col)>();
        stack.Push(first);
        while (stack.TryPop(out var item))
        {
            newSet.Add(item);
            if (set.Contains((item.row - 1, item.col)))
            {
                set.Remove((item.row - 1, item.col));
                stack.Push((item.row - 1, item.col));
            }
            if (set.Contains((item.row + 1, item.col)))
            {
                set.Remove((item.row + 1, item.col));
                stack.Push((item.row + 1, item.col));

            }
            if (set.Contains((item.row, item.col - 1)))
            {
                set.Remove((item.row, item.col - 1));
                stack.Push((item.row, item.col - 1));

            }
            if (set.Contains((item.row, item.col + 1)))
            {
                set.Remove((item.row, item.col + 1));
                stack.Push((item.row, item.col + 1));
            }
        }

        var areaEdges = 0;
        foreach (var (row, col) in newSet)
        {
            if (!newSet.Contains((row - 1, col)))
            {
                areaEdges++;
            }
            if (!newSet.Contains((row + 1, col)))
            {
                areaEdges++;
            }
            if (!newSet.Contains((row, col - 1)))
            {
                areaEdges++;
            }
            if (!newSet.Contains((row, col + 1)))
            {
                areaEdges++;
            }
        }

        var corners = 0; // sides == corners
        foreach (var row in Enumerable.Range(newSet.MinBy(x => x.row).row, newSet.MaxBy(x => x.row).row + 1))
        {
            foreach (var col in Enumerable.Range(newSet.MinBy(x => x.col).col, newSet.MaxBy(x => x.col).col + 1))
            {
                if (newSet.Contains((row, col)))
                {
                    if (!newSet.Contains((row, col - 1)) && !newSet.Contains((row - 1, col)))
                    {
                        corners++;
                    }
                    if (!newSet.Contains((row - 1, col)) && !newSet.Contains((row, col + 1)))
                    {
                        corners++;
                    }
                    if (!newSet.Contains((row, col + 1)) && !newSet.Contains((row + 1, col)))
                    {
                        corners++;
                    }
                    if (!newSet.Contains((row + 1, col)) && !newSet.Contains((row, col - 1)))
                    {
                        corners++;
                    }
                }
                else
                {
                    if (newSet.Contains((row, col - 1)) && newSet.Contains((row - 1, col - 1)) && newSet.Contains((row - 1, col)))
                    {
                        corners++;
                    }
                    if (newSet.Contains((row - 1, col)) && newSet.Contains((row - 1, col + 1)) && newSet.Contains((row, col + 1)))
                    {
                        corners++;
                    }
                    if (newSet.Contains((row, col + 1)) && newSet.Contains((row + 1, col + 1)) && newSet.Contains((row + 1, col)))
                    {
                        corners++;
                    }
                    if (newSet.Contains((row + 1, col)) && newSet.Contains((row + 1, col - 1)) && newSet.Contains((row, col - 1)))
                    {
                        corners++;
                    }
                }
            }
        }

        part1Result += areaEdges * newSet.Count;
        part2Result += corners * newSet.Count;
        // Console.WriteLine((char)('A' + ch) + ": " + corners);
    }
}

Console.WriteLine("Part1: " + part1Result);
Console.WriteLine("Part2: " + part2Result);