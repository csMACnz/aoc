#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

Colorful.Console.WriteAscii("AoC 2025 Day 2");

var testInput = @"11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

Console.WriteLine($"Expected: (1227775554, 4174379265)");
Console.WriteLine($" Example: {Puzzle(testInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/2/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
// Console.WriteLine(content);
Console.WriteLine($"   Day02: {Puzzle(content)}");

static (long one, long two) Puzzle(string input)
{
    var ids = input
        .Split(',')
        .Select(s=>s.Split('-'))
        .Select(l=>(l[0],l[1]))
        .SelectMany(r =>
        {
            var start = long.Parse(r.Item1);
            var end = long.Parse(r.Item2);
            return EnumerableRange(start, end - start + 1);
        })
        .Select(i => (i: (long)i, s: i.ToString("D")));

    var part1 = ids
        .Where(id => id.s.Length  % 2 == 0 && id.s[0..(id.s.Length/2)] == id.s[(id.s.Length/2)..] )
        .Sum(id => id.i);
        
    var part2 = ids
        .Where(id =>
        {
            if (id.s.Length  % 2 == 0 && id.s[0..(id.s.Length/2)] == id.s[(id.s.Length/2)..])
            {
                return true;
            }
            if (id.s.Length  % 3 == 0 && id.s[0..(id.s.Length/3)] == id.s[(id.s.Length/3)..(2*id.s.Length/3)] &&
                id.s[0..(id.s.Length/3)] == id.s[(2*id.s.Length/3)..])
            {
                return true;
            }
            if (id.s.Length  % 4 == 0 && id.s[0..(id.s.Length/4)] == id.s[(id.s.Length/4)..(2*id.s.Length/4)] &&
                id.s[0..(id.s.Length/4)] == id.s[(2*id.s.Length/4)..(3*id.s.Length/4)] &&
                id.s[0..(id.s.Length/4)] == id.s[(3*id.s.Length/4)..])
            {
                return true;
            }
            if (id.s.Length  % 5 == 0 && id.s[0..(id.s.Length/5)] == id.s[(id.s.Length/5)..(2*id.s.Length/5)] &&
                id.s[0..(id.s.Length/5)] == id.s[(2*id.s.Length/5)..(3*id.s.Length/5)] &&
                id.s[0..(id.s.Length/5)] == id.s[(3*id.s.Length/5)..(4*id.s.Length/5)] &&
                id.s[0..(id.s.Length/5)] == id.s[(4*id.s.Length/5)..])
            {
                return true;
            }
            if (id.s.Length  % 6 == 0 && id.s[0..(id.s.Length/6)] == id.s[(id.s.Length/6)..(2*id.s.Length/6)] &&
                id.s[0..(id.s.Length/6)] == id.s[(2*id.s.Length/6)..(3*id.s.Length/6)] &&
                id.s[0..(id.s.Length/6)] == id.s[(3*id.s.Length/6)..(4*id.s.Length/6)] &&
                id.s[0..(id.s.Length/6)] == id.s[(4*id.s.Length/6)..(5*id.s.Length/6)] &&
                id.s[0..(id.s.Length/6)] == id.s[(5*id.s.Length/6)..])
            {
                return true;
            }
            if (id.s.Length  % 7 == 0 && id.s[0..(id.s.Length/7)] == id.s[(id.s.Length/7)..(2*id.s.Length/7)] &&
                id.s[0..(id.s.Length/7)] == id.s[(2*id.s.Length/7)..(3*id.s.Length/7)] &&
                id.s[0..(id.s.Length/7)] == id.s[(3*id.s.Length/7)..(4*id.s.Length/7)] &&
                id.s[0..(id.s.Length/7)] == id.s[(4*id.s.Length/7)..(5*id.s.Length/7)] &&
                id.s[0..(id.s.Length/7)] == id.s[(5*id.s.Length/7)..(6*id.s.Length/7)] &&
                id.s[0..(id.s.Length/7)] == id.s[(6*id.s.Length/7)..])
            {
                return true;
            }
            return false;
        })
        .Sum(id => id.i);
    
    return (part1, part2);
}

static IEnumerable<long> EnumerableRange(long start, long count)
{
    for (long i = 0; i < count; i++)
    {
        yield return start + i;
    }
}