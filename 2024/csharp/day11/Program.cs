// See https://aka.ms/new-console-template for more information
using System.IO.Pipelines;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LongMemoiseCache = System.Collections.Generic.Dictionary<(long value, int repeats), long>;
using BigIntMemoiseCache = System.Collections.Generic.Dictionary<(System.Numerics.BigInteger value, int repeats), System.Numerics.BigInteger>;

Console.WriteLine("Hello, World!");

var input = File.ReadAllText("puzzle.txt").Split().Select(BigInteger.Parse).ToArray();

// BenchmarkRunner.Run<Day11>();

Console.WriteLine("Part1(long): " + new Day11().Part1Long());

Console.WriteLine("Part2(long): " + new Day11().Part2Long());

Console.WriteLine("99(long)=>" + new Day11().NinetyNineLong());

Console.WriteLine("Part1(BigInt): " + new Day11().Part1BigInt());

Console.WriteLine("Part2(BigInt): " + new Day11().Part2BigInt());

Console.WriteLine("97(BigInt)=>" + new Day11().NinetySevenBigInt());

Console.WriteLine("99(BigInt)=>" + new Day11().NinetyNineBigInt());

Console.WriteLine("1000(BigInt)=>" + new Day11().ThousandBigInt());

[MemoryDiagnoser]
public class Day11
{
    [Benchmark]
    public long Part1Long()
    {
        return LongSolve("puzzle.txt", 25);
    }

    [Benchmark]
    public long Part2Long()
    {
        return LongSolve("puzzle.txt", 75);
    }

    [Benchmark]
    public BigInteger Part1BigInt()
    {
        return BigIntegerSolve("puzzle.txt", 25);
    }

    [Benchmark]
    public BigInteger Part2BigInt()
    {
        return BigIntegerSolve("puzzle.txt", 75);
    }

    [Benchmark]
    public long NinetyNineLong()
    {
        return LongSolve("puzzle.txt", 99);
    }

    [Benchmark]
    public BigInteger NinetySevenBigInt()
    {
        return BigIntegerSolve("puzzle.txt", 97);
    }


    [Benchmark]
    public BigInteger NinetyNineBigInt()
    {
        return BigIntegerSolve("puzzle.txt", 99);
    }

    [Benchmark]
    public BigInteger ThousandBigInt()
    {
        return BigIntegerSolve("puzzle.txt", 1000);
    }

    public static long LongSolve(string fileName, int repeats)
    {
        var input = File.ReadAllText("puzzle.txt").Split().Select(long.Parse).ToArray();

        var cache = new LongMemoiseCache();

        return input.Select(n => LongHowMany(cache, n, repeats)).Sum();
    }

    public static BigInteger BigIntegerSolve(string fileName, int repeats)
    {
        var input = File.ReadAllText("puzzle.txt").Split().Select(BigInteger.Parse).ToArray();

        var cache = new BigIntMemoiseCache();

        return input.Select(n => BigIntHowMany(cache, n, repeats)).Aggregate(BigInteger.Zero, (a, b) => a + b);
    }

    private static long LongHowMany(LongMemoiseCache cache, long value, int repeats)
    {
        if (cache.TryGetValue((value, repeats), out var result)) return result;
        result = (repeats, value) switch
        {
            (1, 0) => 1,
            (1, var x) when $"{x}".Length % 2 == 0 => 2,
            (1, _) => 1,
            (_, 0) => LongHowMany(cache, 1, repeats - 1),
            (_, var x) when IsSplitEven(x, out var lhs, out var rhs)
                => LongHowMany(cache, lhs, repeats - 1) + LongHowMany(cache, rhs, repeats - 1),
            _ => LongHowMany(cache, value * 2024, repeats - 1)
        };
        cache[(value, repeats)] = result;
        return result;
    }

    private static BigInteger BigIntHowMany(BigIntMemoiseCache cache, BigInteger value, int repeats)
    {
        if (cache.TryGetValue((value, repeats), out var result)) return result;
        result = (repeats, value) switch
        {
            (1, var z) when z == BigInteger.Zero => 1,
            (1, var x) when $"{x}".Length % 2 == 0 => 2,
            (1, _) => 1,
            (_, var z) when z == BigInteger.Zero => BigIntHowMany(cache, 1, repeats - 1),
            (_, var x) when IsSplitEven(x, out var lhs, out var rhs)
                => BigIntHowMany(cache, lhs, repeats - 1) + BigIntHowMany(cache, rhs, repeats - 1),
            _ => BigIntHowMany(cache, value * 2024, repeats - 1)
        };
        cache[(value, repeats)] = result;
        return result;
    }

    private static bool IsSplitEven(long input, out long lhs, out long rhs)
    {
        if (input.ToString() is string str && str.Length % 2 == 0)
        {
            lhs = long.Parse(str[..(str.Length / 2)]);
            rhs = long.Parse(str[(str.Length / 2)..]);
            return true;
        }
        else
        {
            lhs = rhs = 0;
            return false;
        }
    }
    private static bool IsSplitEven(BigInteger input, out BigInteger lhs, out BigInteger rhs)
    {
        if (input.ToString() is string str && str.Length % 2 == 0)
        {
            lhs = BigInteger.Parse(str[..(str.Length / 2)]);
            rhs = BigInteger.Parse(str[(str.Length / 2)..]);
            return true;
        }
        else
        {
            lhs = rhs = 0;
            return false;
        }
    }
}