// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

var remoteCache = new Dictionary<(char, string), string[]>();
var expandCache = new Dictionary<(char, char), string[]>();

Console.WriteLine("Hello, World!");

var input = File.ReadAllLines("puzzle.txt");

var score = 0;
foreach (var code in input)
{
    string[] instructionsPossibilities = GetKeypadInstructions('A', code)
        .SelectMany(p => FastCacheRemote('A', p)).ToArray();

    var shortest = instructionsPossibilities.MinBy(x => x.Length)!;
    var numberPart = GetNumericPart(code);
    score += shortest.Length * numberPart;

    Console.WriteLine($"{code}({numberPart}): {shortest}({shortest.Length})");
}

Console.WriteLine("Part1: " + score);

string[] GetKeypadInstructions(char ch, ReadOnlySpan<char> code)
{
    if (code.Length is 0) return [""];
    var nextChar = code[0];
    var currentParts = KeyPadSteps(ch, nextChar);
    var nextParts = GetKeypadInstructions(nextChar, code[1..]);
    var results = new List<string>();
    foreach (var lhs in currentParts)
    {
        foreach (var rhs in nextParts)
        {
            results.Add(lhs + rhs);
        }
    }
    var output = results.ToArray();
    return output;
}

static string[] KeyPadSteps(char from, char to)
{
    return (from, to) switch
    {
        ('A', '0') => ["<A"],
        ('A', '1') => ["^<<A", "<^<A",/*gap*/],
        ('A', '2') => ["^<A", "<^A"],
        ('A', '3') => ["^A"],
        ('A', '4') => ["^^<<A", "^<^<A", "^<<^A", "<^^<A", "<^<^A", /*gap*/],
        ('A', '5') => ["^^<A", "<^^A", "^<^A"],
        ('A', '9') => ["^^^A"],
        ('0', '2') => ["^A"],
        ('0', '8') => ["^^^A"],
        ('0', 'A') => [">A"],
        ('1', '7') => ["^^A"],
        ('1', 'A') => [">>vA", ">v>A", "v>>A"],
        ('2', '0') => ["vA"],
        ('2', '9') => [">^^A", "^>^A", "^^>A"],
        ('3', '4') => ["^<<A", "<^<A", "<<^A"],
        ('3', '7') => ["^^<<A", "^<^<A", "^<<^A", "<^^<A", "<^<^A", "<<^^A"],
        ('3', 'A') => ["vA"],
        ('4', '1') => ["vA"],
        ('4', '5') => [">A"],
        ('4', '6') => [">>A"],
        ('5', '6') => [">A"],
        ('5', '8') => ["^A"],
        ('5', '9') => ["^>A", ">^A"],
        ('6', '3') => ["vA"],
        ('6', 'A') => ["vvA"],
        ('7', '9') => [">>A"],
        ('8', '0') => ["vvvA"],
        ('8', '6') => ["v>A", ">vA"],
        ('8', 'A') => ["vvv>A", "vv>vA", "v>vvA", ">vvvA"],
        ('9', '3') => ["vvA"],
        ('9', '8') => ["<A"],
        ('9', 'A') => ["vvvA"],
        _ => throw new UnreachableException("Missing Conbo" + (from, to))
    };
}

string[] FastCacheRemote(char ch, ReadOnlySpan<char> code)
{
    if (code.Length is 0) return [""];
    var currentParts = CachedSingleTwiceExpanded(ch, code[0]);
    var rest = FastCacheRemote(code[0], code[1..]);
    return currentParts.SelectMany(l => rest.Select(r => l + r)).ToArray();
}

string[] CachedSingleTwiceExpanded(char leftCh, char rightCh)
{
    if (expandCache.TryGetValue((leftCh, rightCh), out var cacheResults)) return cacheResults;
    var results = RemoteSteps(leftCh, rightCh).SelectMany(s => GetRemoteInstructions('A', s)).ToArray();
    expandCache.Add((leftCh, rightCh), results);
    return results;
}

string[] GetRemoteInstructions(char ch, ReadOnlySpan<char> code)
{
    if (code.Length is 0) return [""];
    if (remoteCache.TryGetValue((ch, code.ToString()), out var cacheResult)) return cacheResult;
    var nextChar = code[0];
    var currentParts = RemoteSteps(ch, nextChar);
    var nextParts = GetRemoteInstructions(nextChar, code[1..]);
    var results = new List<string>();
    foreach (var lhs in currentParts)
    {
        foreach (var rhs in nextParts)
        {
            results.Add(lhs + rhs);
        }
    }

    var output = results.ToArray();
    remoteCache.Add((ch, code.ToString()), output);
    return output;
}

static string[] RemoteSteps(char from, char to)
{
    return (from, to) switch
    {
        (char a, char b) when a == b => ["A"],
        ('A', '^') => ["<A"],
        ('A', 'v') => ["<vA", "v<A"],
        ('A', '>') => ["vA"],
        ('A', '<') => ["v<<A", "<v<A", "<<vA"],
        ('^', 'v') => ["vA"],
        ('^', '<') => ["v<A" /* gap */],
        ('^', '>') => ["v>A", ">vA"],
        ('^', 'A') => [">A"],
        ('v', '^') => ["^A"],
        ('v', '<') => ["<A"],
        ('v', '>') => [">A"],
        ('v', 'A') => [">^A", "^>A"],
        ('<', '^') => [">^A" /* gap*/],
        ('<', 'v') => [">A"],
        ('<', '>') => [">>A"],
        ('<', 'A') => [">>^A", ">^>A", /*gap*/],
        ('>', '^') => ["<^A", "^<A"],
        ('>', 'v') => ["<A"],
        ('>', '<') => ["<<A"],
        ('>', 'A') => ["^A"],
        _ => throw new UnreachableException("Missing Conbo" + (from, to))
    };
}
static int GetNumericPart(ReadOnlySpan<char> chars)
{
    int val = 0;
    foreach (var ch in chars)
    {
        if (ch is >= '0' and <= '9')
        {
            val = val * 10 + (ch - '0');
        }
    }
    return val;
}