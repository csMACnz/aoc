// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

var expandCache = new Dictionary<(char, char, int), long>();
var depthCache = new Dictionary<(char, char, int), char[][]>();

Console.WriteLine("Hello, World!");

var input = File.ReadAllLines("sample.txt");
var part1Score = Score(2);
Console.WriteLine("Part1: " + part1Score);

foreach (var test in Enumerable.Range(3, 20))
{
    var testAScore = Score(test);
    Console.WriteLine($"{test}==: {testAScore}");
}

var part2Score = Score(25);
Console.WriteLine("Part2: " + part2Score);

long Score(int depth)
{
    var score = 0L;
    foreach (var code in input)
    {
        var shortestLength = GetKeypadInstructions('A', code)
            .Select(p => FastCacheRemote(p, depth)).Min();

        var numberPart = GetNumericPart(code);
        checked
        {
            score += shortestLength * numberPart;
        }
        // Console.WriteLine($"{code}({numberPart}): {shortest}({shortest.Length})");
    }
    return score;
}

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
        ('1', 'A') => [">>vA", ">v>A", /*gap*/],
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

long FastCacheRemote(ReadOnlySpan<char> code, int depth)
{
    var length = 0L;
    var index = 0;
    var lastChar = 'A';
    while (index < code.Length)
    {
        var currentCh = code[index];
        var shortestLength = CachedSingleExpanded(lastChar, currentCh, depth);
        length += shortestLength;
        index++;
        lastChar = currentCh;
    }
    return length;
}

long CachedSingleExpanded(char leftCh, char rightCh, int depth)
{
    if (expandCache.TryGetValue((leftCh, rightCh, depth), out var cacheResult)) return cacheResult;
    var results = GetRemoteInstructions2(leftCh, rightCh, depth);
    var shortestLength = results.MinBy(s => s.Length)!.Length;
    expandCache.Add((leftCh, rightCh, depth), shortestLength);
    return shortestLength;
}

char[][] GetRemoteInstructions(ReadOnlySpan<char> code, int depth)
{
    char[][] results = [[]];
    var index = 0;
    var lastChar = 'A';
    while (index < code.Length)
    {
        Console.Write('X');
        var currentCh = code[index];
        var nextResults = GetRemoteInstructions2(lastChar, currentCh, depth);
        results = results.SelectMany(s => nextResults.Select<char[], char[]>(n => [.. s, .. n])).ToArray();
        index++;
        lastChar = currentCh;
    }
    return results;
}

char[][] GetRemoteInstructions2(char fromCh, char toCh, int depth)
{
    if (depthCache.TryGetValue((fromCh, toCh, depth), out var cacheResult)) return cacheResult;
    var results = RemoteSteps(fromCh, toCh).Select(s => s.ToCharArray()).ToArray();
    if (depth > 1)
    {
        results = results.SelectMany(r => GetRemoteInstructions(r, depth - 1)).ToArray();
    }
    // var minLength = results.MinBy(x => x.Length)!.Length;
    // results = results.Where(x => x.Length == minLength).ToArray();
    results = [results.MinBy(x => x.Length)!];
    depthCache[(fromCh, toCh, depth)] = results;
    return results;
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
static long GetNumericPart(ReadOnlySpan<char> chars)
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