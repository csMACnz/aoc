// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Ruleset = System.Collections.Generic.Dictionary<string, (string op, string lhs, string rhs)>;
using OutputCache = System.Collections.Generic.Dictionary<string, bool>;

Console.WriteLine("Hello, World!");

var rules = ParseRules("puzzle.txt");
var part1Cache = new OutputCache();
var zKeys = rules.Keys.Where(k => k.StartsWith('z')).Order().ToList();
var part1 = BinaryNumberFor(rules, part1Cache, zKeys);
Console.WriteLine("Part1: " + part1);

var x = BinaryNumberFor(rules, part1Cache, rules.Keys.Where(k => k.StartsWith('x')));
var y = BinaryNumberFor(rules, part1Cache, rules.Keys.Where(k => k.StartsWith('y')));

Console.WriteLine(x + " + " + y + "==" + (x + y));
Console.WriteLine($"{part1:b}");
Console.WriteLine($"{(x + y):b}");

Test2(rules);

static Ruleset ParseRules(string filename)
{
    var lines = File.ReadAllLines(filename);
    Ruleset rules = [];
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
            rules[line[..3]] = (line[5] == '1' ? "TRUE" : "FALSE", null!, null!);
        }
    }
    return rules;
}

static long BinaryNumberFor(Ruleset rules, OutputCache cache, IEnumerable<string> keys)
{
    long result = 0;
    foreach (var n in keys.OrderBy(n => int.Parse(n[1..])))
    {
        var move = int.Parse(n[1..]);
        var add = Resolve(rules, cache, n);

        // Console.WriteLine(n + "==" + add);
        if (add)
        {
            result += 1L << move;
        }
    }
    return result;
}

static bool BitDigitFromAt(long number, int index)
{
    return (number >> index) % 2 == 1;
}

static bool Resolve(Ruleset rules, OutputCache cache, string key)
{
    if (cache.TryGetValue(key, out var isOn)) return isOn;
    var (op, lhs, rhs) = rules[key];
    var result = op switch
    {
        "TRUE" => true,
        "FALSE" => false,
        "AND" => Resolve(rules, cache, lhs) && Resolve(rules, cache, rhs),
        "OR" => Resolve(rules, cache, lhs) || Resolve(rules, cache, rhs),
        "XOR" => Resolve(rules, cache, lhs) != Resolve(rules, cache, rhs),
        _ => throw new UnreachableException(key)
    };
    cache[key] = result;
    return result;
}


static bool? TryResolve(Ruleset rules, OutputCache cache, string key, int depth = 0)
{
    if (depth > 500) return null; //loop
    if (cache.TryGetValue(key, out var isOn)) return isOn;
    var (op, lhs, rhs) = rules[key];
    bool? result = op switch
    {
        "TRUE" => true,
        "FALSE" => false,
        "AND" => TryResolve(rules, cache, lhs, depth + 1) is bool one && TryResolve(rules, cache, rhs, depth + 1) is bool two ? (one && two) : null,
        "OR" => TryResolve(rules, cache, lhs, depth + 1) is bool one && TryResolve(rules, cache, rhs, depth + 1) is bool two ? (one || two) : null,
        "XOR" => TryResolve(rules, cache, lhs, depth + 1) is bool one && TryResolve(rules, cache, rhs, depth + 1) is bool two ? (one != two) : null,
        _ => throw new UnreachableException(key)
    };
    if (result.HasValue)
    {
        cache[key] = result.Value;
    }
    return result;
}

static IEnumerable<string> FindContributors(Ruleset rules, string zkey)
{
    var (op, lhs, rhs) = rules[zkey];
    IEnumerable<string> result = [zkey];
    if (lhs[0] != 'x' && lhs[0] != 'y')
    {
        result = result.Concat(FindContributors(rules, lhs));
    }
    if (rhs[0] != 'x' && rhs[0] != 'y')
    {
        result = result.Concat(FindContributors(rules, rhs));
    }
    return result;
}
static IEnumerable<string> FindXYContributors(Ruleset rules, string key)
{
    if (key[0] == 'x' || key[0] == 'y')
    {
        return [key];
    }
    var (op, lhs, rhs) = rules[key];
    return [.. FindXYContributors(rules, lhs), .. FindXYContributors(rules, rhs)];
}

static void Test2(Ruleset rules)
{
    var zKeys = rules.Keys.Where(k => k[0] == 'z').Order().ToList();
    Console.WriteLine("ZCount = " + zKeys.Count);
    HashSet<string> safeRules = [];
    var index = 0;
    var bitCount = zKeys.Count;
    var results = new List<string>();
    while (index < bitCount)
    {
        var safe = CheckIsSafe(rules, index);
        if (safe)
        {
            Console.WriteLine($"z{index:D2} SAFE");
            if (index >= 2)
            {
                foreach (var rule in FindContributors(rules, $"z{index - 2:D2}"))
                {
                    safeRules.Add(rule);
                }
            }
            var testCache = new OutputCache();
            var testOutput = BinaryNumberFor(rules, testCache, zKeys);

            var x = BinaryNumberFor(rules, testCache, rules.Keys.Where(k => k.StartsWith('x')));
            var y = BinaryNumberFor(rules, testCache, rules.Keys.Where(k => k.StartsWith('y')));

            Console.WriteLine($"{x} + {y} == {x + y} (GOT {testOutput})");
            Console.WriteLine($"{testOutput:b}");
            Console.WriteLine($"{(x + y):b}");

            index++;
        }
        else
        {
            Console.WriteLine($"z{index:D2} UNSAFE::SEARCHING FOR SWAP");
            var candidates = FindContributors(rules, $"z{index:D2}")
                .Concat(FindContributors(rules, $"z{int.Min(index + 1, bitCount - 1):D2}"))
                .Concat(FindContributors(rules, $"z{int.Min(index + 2, bitCount - 1):D2}"))
                .Distinct()
                .Except(safeRules)
                .ToArray();
            // candidates = rules.Keys.Where(k => k[0] != 'x' && k[0] != 'y').ToArray();
            var breakEarly = false;
            foreach (var i in Enumerable.Range(0, candidates.Length - 1))
            {
                foreach (var j in Enumerable.Range(i + 1, candidates.Length - i - 1))
                {
                    if (candidates[i][0] == 'z' && candidates[j][0] == 'z') continue;
                    var newRules = rules.ToDictionary();
                    newRules[candidates[i]] = rules[candidates[j]];
                    newRules[candidates[j]] = rules[candidates[i]];
                    if (Enumerable.Range(0, index + 1).All(i => CheckIsSafe(newRules, i)))
                    {
                        Console.WriteLine(candidates[i] + "=>" + rules[candidates[i]]);
                        Console.WriteLine(candidates[j] + "=>" + rules[candidates[j]]);
                        Console.WriteLine(candidates[i] + "," + candidates[j]);
                        Console.WriteLine(candidates[i] + ":" + string.Join(",", FindXYContributors(newRules, candidates[i]).Distinct().Order()));
                        Console.WriteLine(candidates[j] + ":" + string.Join(",", FindXYContributors(newRules, candidates[j]).Distinct().Order()));
                        results.Add(candidates[i]);
                        results.Add(candidates[j]);
                        rules = newRules;
                        breakEarly = true;
                        break;
                    }
                }
                if (breakEarly) break;
            }
            if (!breakEarly) throw new UnreachableException("should find swap");
        }
    }
    Console.WriteLine("Expect cph,gws,hgj,nnt,npf,z13,z19,z33");
    Console.WriteLine("Found  " + string.Join(",", results.Order()));
}

static Ruleset SetInput(Ruleset rules, long x, long y)
{
    var newRules = rules.ToDictionary();
    foreach (var key in rules.Keys.Where(k => k[0] == 'x' || k[0] == 'y'))
    {
        newRules[key] = (BitDigitFromAt(key[0] == 'x' ? x : y, int.Parse(key[1..])) ? "TRUE" : "FALSE", null!, null!);
    }
    return newRules;
}

static bool CheckIsSafe(Ruleset rules, int index)
{
    (long x, long y)[] bits = index == 0
    ? [(0, 0), (0, 1), (1, 0), (1, 1)]
    : index == 45 ? [(0b00, 0b00), (0b00, 0b01), (0b01, 0b00), (0b01, 0b01)]
    : [(0b00, 0b00), (0b00, 0b01), (0b00, 0b10), (0b00, 0b11), (0b01, 0b00), (0b01, 0b01), (0b01, 0b10), (0b01, 0b11), (0b10, 0b00), (0b10, 0b01), (0b10, 0b10), (0b10, 0b11), (0b11, 0b00), (0b11, 0b01), (0b11, 0b10), (0b11, 0b11)];
    var shift = index == 0 ? 0 : index - 1;
    var safe = true;
    foreach (var xy in bits)
    {
        var newRules = SetInput(rules, xy.x << shift, xy.y << shift);
        var cache = new OutputCache();
        bool? zResult = TryResolve(newRules, cache, $"z{index:D2}");
        if (!zResult.HasValue) return false;
        long expected = (xy.x + xy.y) % (index == 0 ? 2 : 4);
        var resolved = zResult.Value ? 1 : 0;
        if (index > 0)
        {
            var zMinusResult = TryResolve(newRules, cache, $"z{index - 1:D2}");
            if (!zMinusResult.HasValue) return false;
            resolved = ((zResult.Value ? 1 : 0) << 1) + (zMinusResult.Value ? 1 : 0);
        }
        if (resolved != expected)
        {
            if (index == 45)
            {
                // Console.WriteLine($"problem on {index} ({xy.x}+{xy.y}=={expected} != {resolved})");
            }

            if (index >= 31)
            {
                // Console.WriteLine($"problem on {index} ({xy.x}+{xy.y}=={expected} != {resolved})");
            }
            safe = false;
            break;
        }
    }
    return safe;
}