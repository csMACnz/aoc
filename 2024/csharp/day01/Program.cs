// See https://aka.ms/new-console-template for more information
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

// BenchmarkRunner.Run<DayOne>();
new DayOne().FirstAttempt();
new DayOne().FasterAttempt();

[MemoryDiagnoser]
public class DayOne
{
    [Benchmark]
    public void FirstAttempt()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var lines = File.ReadLines("puzzle.txt");

        var leftList = new List<int>();
        var rightList = new List<int>();
        foreach (var line in lines)
        {
            var l = line.Split("   ");
            var a = int.Parse(l[0]);
            var b = int.Parse(l[1]);
            leftList.Add(a);
            rightList.Add(b);
        }
        leftList.Sort();
        rightList.Sort();

        var result1a = leftList.Zip(rightList).Select(((int l, int r) x) => Math.Abs(x.l - x.r)).Sum();

        Console.WriteLine($"part 1 is: {result1a}");

        // Part 2
        var memo = new Dictionary<int, int>();
        var result2a = 0;
        foreach (var number in leftList)
        {
            if (!memo.ContainsKey(number))
            {
                memo[number] = number * rightList.Count(b => b == number);
            }
            result2a += memo[number];
        }

        Console.WriteLine($"part 2 is: {result2a}");
        // the code that you want to measure comes here
        watch.Stop();
        Console.WriteLine($"first attempt: {watch.ElapsedMilliseconds}ms");
    }

    [Benchmark]
    public void FasterAttempt()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var lines = File.ReadLines("puzzle.txt");

        var left = new SortedList<int, int>();
        var right = new SortedList<int, int>();
        foreach (var line in lines)
        {
            var l = line.Split("   ");
            var a = int.Parse(l[0]);
            var b = int.Parse(l[1]);
            AddorIncrement(left, a);
            AddorIncrement(right, b);
        }

        var result1b = Iterate(left).Zip(Iterate(right)).Select(((int l, int r) x) => Math.Abs(x.l - x.r)).Sum();

        Console.WriteLine($"part 1 is: {result1b}");

        // Part 2
        var result2b = 0;
        foreach (var kvp in left)
        {
            if (right.TryGetValue(kvp.Key, out var rhs))
            {
                result2b += kvp.Value * kvp.Key * rhs;
            }
        }

        Console.WriteLine($"part 2 is: {result2b}");
        // the code that you want to measure comes here
        watch.Stop();
        Console.WriteLine($"second attempt: {watch.ElapsedMilliseconds}ms");
    }

    static IEnumerable<int> Iterate(SortedList<int, int> data)
    {
        foreach (var kvp in data)
        {
            for (var i = 0; i < kvp.Value; i++)
            {
                yield return kvp.Key;
            }
        }
    }

    static void AddorIncrement(SortedList<int, int> data, int value)
    {
        if (!data.ContainsKey(value))
        {
            data[value] = 1;
        }
        else
        {
            data[value]++;
        }
    }

}