// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var dimensions = File.ReadAllLines("puzzle.txt")
.Select(l => l.Split('x'))
.Select(x => (l: int.Parse(x[0]), w: int.Parse(x[1]), h: int.Parse(x[2])))
.ToArray();

var part1 = dimensions
.Select(x => new[] { x.l * x.w, x.w * x.h, x.h * x.l })
.Sum(arr => 2 * arr.Sum() + arr.Min());

Console.WriteLine("Part1: " + part1);

var part2 = dimensions
    .Sum(x => (x.l * x.h * x.w) + (2 * (x.l + x.h + x.w)) - (2 * int.Max(x.l, int.Max(x.w, x.h))));

Console.WriteLine("Part2: " + part2);