// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var instructions = new Dictionary<string, (string op, string in1, string in2)>();
var lines = File.ReadAllLines("puzzle.txt");
foreach (var line in lines)
{
    var (key, value) = Parse(line);
    instructions.Add(key, value);
}

var output = new Dictionary<string, long>();

var part1 = Get(instructions, output, "a");
Console.WriteLine("Part1: " + part1);

var part2Output = new Dictionary<string, long> { { "b", 16076 } };
var part2 = Get(instructions, part2Output, "a");
Console.WriteLine("Part2: " + part2);

static (string key, (string op, string in1, string in2)) Parse(ReadOnlySpan<char> line)
{
    var parts = line.Split(" ");
    parts.MoveNext();
    var one = line[parts.Current];
    parts.MoveNext();
    var two = line[parts.Current];
    parts.MoveNext();
    var three = line[parts.Current];
    if (parts.MoveNext())
    {
        var four = line[parts.Current];
        if (parts.MoveNext())
        {
            var key = line[parts.Current].ToString();
            if (parts.MoveNext()) throw new UnreachableException(line.ToString() + ":" + $"{one} {two} {three} {four} {key} {line[parts.Current]}");
            if (four.ToString() != "->") throw new UnreachableException(line.ToString() + ":" + four.ToString());
            return (key, (two.ToString(), one.ToString(), three.ToString()));
        }
        else
        {
            if (three.ToString() != "->") throw new UnreachableException(line.ToString());
            return (four.ToString(), (one.ToString(), two.ToString(), ""));
        }
    }
    else
    {
        if (two.ToString() != "->") throw new UnreachableException(line.ToString());
        return (three.ToString(), ("CONST", one.ToString(), ""));
    }
}

static long Get(Dictionary<string, (string op, string in1, string in2)> instructions, Dictionary<string, long> output, string key)
{
    if (output.TryGetValue(key, out var result)) return result;
    if (long.TryParse(key, out var value))
    {
        return value;
    }
    var outcome = instructions[key] switch
    {
        ("CONST", string val, _) => Get(instructions, output, val),
        ("AND", string in1, string in2) => Get(instructions, output, in1) & Get(instructions, output, in2),
        ("OR", string in1, string in2) => Get(instructions, output, in1) | Get(instructions, output, in2),
        ("LSHIFT", string in1, string in2) => Get(instructions, output, in1) << int.Parse(in2),
        ("RSHIFT", string in1, string in2) => Get(instructions, output, in1) >> int.Parse(in2),
        ("NOT", string val, _) => ~Get(instructions, output, val),
        var x => throw new UnreachableException(key + ":" + x)
    };
    output[key] = outcome;
    return outcome;
}