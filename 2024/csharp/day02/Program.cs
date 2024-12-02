// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day02>();

[MemoryDiagnoser]
public class Day02
{
    [Benchmark]
    public void Part1()
    {
        var lines = File.ReadAllLines("puzzle.txt");
        var count = 0;
        foreach (var line in lines)
        {
            var nums = ParseLevels(line);
            var first = nums[0];
            var matches = growingCheck(nums, []) || shrinkingCheck(nums, []);
            if (matches)
            {
                count++;
            }
        }

        // Console.WriteLine($"part 1: {count}");
    }

    [Benchmark]
    public void Part2()
    {
        var lines = File.ReadAllLines("puzzle.txt");
        var count = 0;
        foreach (var line in lines)
        {
            var nums = ParseLevels(line);
            var first = nums[0];
            var matches = growingCheck(nums, []) || shrinkingCheck(nums, []);
            if (!matches)
            {
                for (var i = 0; i < nums.Length; i++)
                {
                    var lhs = nums[0..i];
                    var rhs = nums[(i + 1)..];

                    matches = growingCheck(lhs, rhs) || shrinkingCheck(lhs, rhs);
                    if (matches)
                    {
                        break;
                    }
                }
            }
            if (matches)
            {
                count++;
            }
        }

        // Console.WriteLine($"part 2: {count}");
    }

    private static Span<int> ParseLevels(ReadOnlySpan<char> line)
    {
        int[] results = new int[line.Length];
        var count = 0;
        foreach (var number in line.Split(' '))
        {
            results[count] =int.Parse(line[number]);
            count++;
        }
        return results.AsSpan()[..count];
    }

    private static bool growingCheck(Span<int> nums, Span<int> moreNums)
    {
        return AllPairs(nums, moreNums, (a, b) => a < b && b - a <= 3);
    }


    private static bool shrinkingCheck(Span<int> nums, Span<int> moreNums)
    {
        return AllPairs(nums, moreNums, (a, b) => a > b && a - b <= 3);
    }

    private static bool AllPairs(Span<int> nums, Span<int> moreNums, Func<int, int, bool> check)
    {
        var iter = nums.GetEnumerator();
        int? prev = null;
        if (iter.MoveNext())
        {
            prev = iter.Current;
            while (iter.MoveNext())
            {
                var curr = iter.Current;
                if (!check(prev.Value, curr))
                {
                    return false;
                }
                prev = curr;
            }
        }
        iter = moreNums.GetEnumerator();
        if (prev is null)
        {
            iter.MoveNext();
            prev = iter.Current;
        }
        while (iter.MoveNext())
        {
            var curr = iter.Current;
            if (!check(prev.Value, curr))
            {
                return false;
            }
            prev = curr;
        }
        return true;
    }
}