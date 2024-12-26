// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var index = 0;
var locks = new List<int[]>();
var keys = new List<int[]>();
while (index < lines.Length)
{
    if (lines[index].All(x => x is '#'))
    {
        // Is Lock
        locks.Add(Parse(lines[(index + 1)..(index + 7)]));
    }
    else if (lines[index + 6].All(x => x is '#'))
    {
        //Is Key
        keys.Add(Parse(lines[index..(index + 6)]));
    }
    else
    {
        throw new UnreachableException("not key or lock");
    }
    index += 8;
}
var part1Count = 0;
foreach (var l in locks)
{
    foreach (var k in keys)
    {
        if (Add(l, k).All(v => v <= 5))
        {
            part1Count++;
            // Console.WriteLine($"Key {ToString(k)} & Lock {ToString(l)} Compatible");
        }
    }
}
Console.WriteLine("Part1: " + part1Count);

static int[] Parse(IEnumerable<string> lines)
{
    return lines
    .Select(l => l.Select(ch => ch is '#' ? 1 : 0).ToArray())
    .Aggregate<int[], int[]>([0, 0, 0, 0, 0], Add);
}

static int[] Add(int[] a, int[] b)
{
    return [a[0] + b[0], a[1] + b[1], a[2] + b[2], a[3] + b[3], a[4] + b[4]];
}

static string ToString(int[] x)
{
    return "[" + string.Join(",", x) + "]";
}