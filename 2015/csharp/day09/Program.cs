// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var distances = File.ReadLines("puzzle.txt")
.SelectMany<string, Edge>(l =>
{
    //AlphaCentauri to Snowdin = 66
    var parts = l.Split(" ");
    var from = parts[0];
    var to = parts[2];
    var distance = long.Parse(parts[4]);
    return [new Edge(from, to, distance), new Edge(to, from, distance)];
}).ToArray();

var destinations = distances.SelectMany<Edge, string>(d => [d.From, d.To]).Distinct().ToArray();
var queue = new PriorityQueue<(string current, string[] remainder), long>();
foreach (var destination in destinations)
{
    queue.Enqueue((destination, destinations.Where(d => d != destination).ToArray()), 0L);
}

var allPaths = new List<long>();
while (queue.TryDequeue(out var item, out long score))
{
    if (item.remainder.Length == 0)
    {
        allPaths.Add(score);
    }
    else
    {
        foreach (var next in item.remainder.Select(d => distances.FirstOrDefault(e => e.From == item.current && e.To == d)))
        {
            if (next is not null)
            {
                queue.Enqueue((next.To, item.remainder.Where(d => d != next.To).ToArray()), score + next.Distance);
            }
        }
    }
}

Console.WriteLine("Part1: " + allPaths.Min());
Console.WriteLine("Part2: " + allPaths.Max());

record class Edge(string From, string To, long Distance);