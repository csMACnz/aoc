// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

Dictionary<char, List<string>> teeData = [];
HashSet<(string, string)> all = [];
HashSet<string> distinct = [];
foreach (var line in lines)
{
    var left = line[..2];
    var right = line[3..5];
    distinct.Add(left);
    distinct.Add(right);
    all.Add((left, right));
    all.Add((right, left));
    if (left.StartsWith('t'))
    {
        if (teeData.TryGetValue(left[1], out var list))
        {
            list.Add(right);
        }
        else
        {
            teeData[left[1]] = [right];
        }
    }
    if (right.StartsWith('t'))
    {
        if (teeData.TryGetValue(right[1], out var list))
        {
            list.Add(left);
        }
        else
        {
            teeData[right[1]] = [left];
        }
    }
}
List<(string, string, string)> teeTriads = [];
foreach (var t in teeData.Keys)
{
    var list = teeData[t];
    foreach (var i in Enumerable.Range(0, list.Count - 1))
    {
        if (list[i][0] == 't' && list[i][1] < t) continue;
        foreach (var j in Enumerable.Range(i + 1, list.Count - i - 1))
        {
            if (all.Contains((list[i], list[j])))
            {
                if (list[j][0] == 't' && list[j][1] < t) continue;
                teeTriads.Add(($"t{t}", list[i], list[j]));
            }
        }
    }
}

Console.WriteLine("Node count: " + distinct.Count);
Console.WriteLine("part1: " + teeTriads.Count);

// all is graph edges, walk the graph?
HashSet<string> largestSet = [];
foreach (var start in distinct)
{
    if (largestSet.Contains(start)) continue;
    var set = new HashSet<string> { start };
    foreach (var node in distinct)
    {
        if (set.Contains(node)) continue;
        if (set.All(s => all.Contains((node, s))))
        {
            set.Add(node);
        }
    }
    if (set.Count > largestSet.Count)
    {
        largestSet = set;
    }
}

var password = string.Join(',', largestSet.Order());
Console.WriteLine("part2: " + password);