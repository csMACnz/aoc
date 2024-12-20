﻿// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var towels = lines[0].Split(", ")!;
var patterns = lines.Skip(2).ToArray();

// var regex = "^(" + string.Join("|", towels) + ")+$";
// var reg = new Regex(regex);
// var count = 0;

// foreach (var pattern in patterns)
// {
//     Console.Write(".");
//     if (reg.IsMatch(pattern))
//     {
//         count++;
//     }
// }
// Console.WriteLine();
// Console.WriteLine("Part1: " + count);


var towelHash = towels.ToHashSet();

var lookup = towelHash.GetAlternateLookup<ReadOnlySpan<char>>();

var minTowelLength = towels.MinBy(static x => x.Length).Length;

var maxTowelLength = towels.MaxBy(static x => x.Length).Length;

var possible = new List<string>();

var patternCache = new Dictionary<string, bool>();
var patternCacheLookup = patternCache.GetAlternateLookup<ReadOnlySpan<char>>();
foreach (var pattern in patterns)
{
    Console.Write(".");
    if (PatternIsValid(pattern.AsSpan()))
    {
        possible.Add(pattern);
    }
}
Console.WriteLine();
Console.WriteLine("Part1: " + possible.Count);

bool PatternIsValid(ReadOnlySpan<char> pattern)
{
    if (patternCacheLookup.TryGetValue(pattern, out var found)) return found;
    if (pattern.Length == 0) return true;
    foreach (var size in Enumerable.Range(minTowelLength, maxTowelLength).Reverse())
    {
        if (pattern.Length >= size)
        {
            if (lookup.Contains(pattern[..size]) && PatternIsValid(pattern[size..]))
            {
                patternCache.Add(pattern.ToString(), true);
                return true;
            }
        }
    }
    patternCache.Add(pattern.ToString(), false);
    return false;
}