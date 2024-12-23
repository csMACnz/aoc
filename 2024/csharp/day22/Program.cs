// See https://aka.ms/new-console-template for more information
using System.Data;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var inputs = File.ReadAllLines("puzzle.txt").Select(long.Parse).ToArray();

var part1Result = Part1(inputs);
Console.WriteLine("Part1: " + part1Result);

var part2Result = Part2(inputs);
Console.WriteLine("Part2: " + part2Result);

static long Part2(long[] inputs)
{
    var options = GetPossibilities(inputs);
    var bestScore = options.Max(static o => o.Value.Values.Sum(static v => v));
    return bestScore;
}

static Dictionary<(short a, short b, short c, short d), Dictionary<int, short>> GetPossibilities(long[] inputs)
{
    Dictionary<(short a, short b, short c, short d), Dictionary<int, short>> result = [];
    foreach (var (index, input) in inputs.Index())
    {
        var diffs = new List<short>();
        var prev = input;
        foreach (var iteration in Enumerable.Range(0, 2000))
        {
            var end = Psuedo(prev);
            short endPrice = (short)(end % 10);
            short diff = (short)(endPrice - (prev % 10));
            diffs.Add(diff);
            if (diffs.Count == 5)
            {
                diffs.RemoveAt(0);
            }
            if (diffs.Count > 4) throw new UnreachableException("bad option");
            if (diffs.Count == 4)
            {
                var key = (diffs[0], diffs[1], diffs[2], diffs[3]);
                if (result.TryGetValue(key, out var dict))
                {
                    if (!dict.ContainsKey(index))
                    {
                        dict[index] = endPrice;
                    }
                }
                else
                {
                    result[key] = new Dictionary<int, short> { { index, endPrice } };
                }
            }
            prev = end;
        }
    }
    return result;
}

static long Part1(long[] inputs)
{
    var result = 0L;
    foreach (var input in inputs)
    {
        var end = puzzle(input, 2000);
        // Console.WriteLine($"{input}: {end}");
        checked
        {
            result += end;
        }
    }
    return result;
}

static long puzzle(long seed, int iterations)
{
    long result = seed;
    foreach (var iteration in Enumerable.Range(0, iterations))
    {
        result = Psuedo(result);
    }
    return result;
}

static long Psuedo(long seed)
{
    var shift = seed << 6; // * 64 == * 2^6 == << 6
    var result = (seed ^ shift) % 16777216;
    shift = result >> 5; // / 32 == / 2^5 == >> 5
    result = (result ^ shift) % 16777216;
    shift = result << 11; // * 2048 == * 2^11 == << 11
    result = (result ^ shift) % 16777216;

    return result;
}