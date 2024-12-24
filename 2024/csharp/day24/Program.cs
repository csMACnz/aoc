// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
Dictionary<string, bool> output = [];
Dictionary<string, (string op, string lhs, string rhs)> rules = [];
bool afterBlank = false;
foreach (var line in lines)
{
    if (afterBlank)
    {
        var split = line.Split(" -> ");
        var key = split[1];
        var rule = split[0].Split(" ");
        var lhs = rule[0];
        var rhs = rule[2];
        var op = rule[1];
        rules[key] = (op, lhs, rhs);

        // Console.WriteLine(key + "==" + lhs + " " + op + " " + rhs);

    }
    else if (line == string.Empty)
    {
        afterBlank = true;
    }
    else
    {
        output[line[..3]] = line[5] == '1';
        // Console.WriteLine(line[..3] + "==" + line[5]);
    }
}

long result = 0;
foreach (var z in rules.Keys.Where(k => k.StartsWith('z')).OrderBy(z => int.Parse(z[1..])))
{
    var move = int.Parse(z[1..]);
    var add = Resolve(z);

    // Console.WriteLine(z + "==" + add);
    if (add)
    {
        result += 1L << move;
    }
}
Console.WriteLine("Part1: " + result);

bool Resolve(string key)
{
    if (output.TryGetValue(key, out var isOn)) return isOn;
    var (op, lhs, rhs) = rules[key];
    var result = op switch
    {
        "AND" => Resolve(lhs) && Resolve(rhs),
        "OR" => Resolve(lhs) || Resolve(rhs),
        "XOR" => Resolve(lhs) != Resolve(rhs),
        _ => throw new UnreachableException(key)
    };
    output[key] = result;
    return result;
}
