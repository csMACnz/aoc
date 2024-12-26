// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var instructions = File.ReadAllText("puzzle.txt");

var start = (x: 0, y: 0);

var part1Pos = start;
HashSet<(int x, int y)> p1Visited = [start];

var santaPos = start;
var roboPos = start;
HashSet<(int x, int y)> p2Visited = [start];

foreach (var (index, dir) in instructions.Index())
{
    (int x, int y) = dir switch
    {
        '<' => (-1, 0),
        '>' => (1, 0),
        '^' => (0, -1),
        'v' => (0, 1),
        _ => throw new UnreachableException("" + dir)
    };
    part1Pos = (part1Pos.x + x, part1Pos.y + y);
    p1Visited.Add(part1Pos);

    if (index % 2 is 0)
    {
        santaPos = (santaPos.x + x, santaPos.y + y);
        p2Visited.Add(santaPos);
    }
    else
    {
        roboPos = (roboPos.x + x, roboPos.y + y);
        p2Visited.Add(roboPos);

    }
}

var part1 = p1Visited.Count;
Console.WriteLine("Part1: " + part1);

var part2 = p2Visited.Count;
Console.WriteLine("Part2: " + part2);