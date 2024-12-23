// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

Dictionary<char, List<string>> teeData = [];
HashSet<(string, string)> all = [];
foreach (var line in lines)
{
    var left = line[..2];
    var right = line[3..5];
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
var part1Result = 0;
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
                part1Result++;
            }
        }
    }
}
Console.WriteLine("part1: " + part1Result);