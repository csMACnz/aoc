﻿// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var input = "puzzle.txt";
var dim = input == "puzzle.txt" ? 71 : 7;
var part1Limit = input == "puzzle.txt" ? 1024 : 12;

var fallData = File.ReadLines(input).Select(l =>
{
    var parts = l.AsSpan().Split(",");
    parts.MoveNext();
    var x = l[parts.Current];
    parts.MoveNext();
    var y = l[parts.Current];
    return (int.Parse(x), int.Parse(y));
}).ToList();
var fallDataHash = fallData.Take(part1Limit).ToHashSet();

var queue = new Queue<((int X, int Y) pos, int score)>();
queue.Enqueue(((0, 0), 0));
var seen = new HashSet<(int x, int y)>();
while (queue.TryDequeue(out var item))
{
    if (seen.Contains(item.pos)) continue;
    seen.Add(item.pos);
    if (item.pos == (dim-1, dim-1))
    {
        // at end
        Console.WriteLine($"Part1: {item.score}");
        break;
    }
    if (item.pos.X < 0 || item.pos.X >= dim || item.pos.Y < 0 || item.pos.Y >= dim)
    {
        // out of bounds
        continue;
    }
    if (fallDataHash.Contains(item.pos))
    {
        // blocked by rock
        continue;
    }
    queue.Enqueue(((item.pos.X + 1, item.pos.Y), item.score + 1));
    queue.Enqueue(((item.pos.X - 1, item.pos.Y), item.score + 1));
    queue.Enqueue(((item.pos.X, item.pos.Y + 1), item.score + 1));
    queue.Enqueue(((item.pos.X, item.pos.Y - 1), item.score + 1));
}