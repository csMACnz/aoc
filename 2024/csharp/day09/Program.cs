// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dumpify;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day09>();

Console.WriteLine($"Part1: {new Day09().Part1()}");
Console.WriteLine($"Part2: {new Day09().Part2()}");

[MemoryDiagnoser]
public class Day09
{

    [Benchmark]
    public long Part1()
    {
        var line = File.ReadAllText("puzzle.txt");

        var data = new List<short>();
        short valueIndex = 0;
        var onGap = false;
        foreach (var ch in line)
        {
            var value = short.Parse([ch]);
            foreach (var _ in Enumerable.Range(0, value))
            {
                data.Add(onGap ? short.MinValue : valueIndex);
            }
            if (!onGap)
            {
                valueIndex++;
            }
            onGap = !onGap;
        }
        long sum = 0;
        var readIndex = 0;
        while (data.Count != 0)
        {
            short value = data[0];
            data.RemoveAt(0);

            if (value == short.MinValue)
            {
                while (value == short.MinValue)
                {
                    if (data.Count == 0) break;
                    value = data[^1];
                    data.RemoveAt(data.Count - 1);
                }
                if (data.Count == 0) break;
            }
            sum += value * readIndex;
            readIndex++;
        }

        return sum;
    }

    [Benchmark]
    public long Part2()
    {
        var line = File.ReadAllText("puzzle.txt");

        var data = new List<short>();
        List<(int offset, int size)> gaps = [];
        List<(int offset, int size, short value)> values = [];
        short valueIndex = 0;
        var onGap = false;
        foreach (var ch in line)
        {
            var value = short.Parse([ch]);
            if (value > 0)
            {
                if (onGap)
                {
                    gaps.Add((data.Count, value));
                }
                else
                {
                    values.Insert(0, (data.Count, value, valueIndex));
                }
            }
            foreach (var _ in Enumerable.Range(0, value))
            {
                data.Add(onGap ? short.MinValue : valueIndex);
            }
            if (!onGap)
            {
                valueIndex++;
            }
            onGap = !onGap;
        }

        foreach (var value in values)
        {
            foreach (var gapIndex in Enumerable.Range(0, gaps.Count))
            {
                var gap = gaps[gapIndex];
                if (gap.offset > value.offset) break;
                if (gap.size >= value.size)
                {
                    for (var i = 0; i < value.size; i++)
                    {
                        data[gap.offset + i] = value.value;
                        data[value.offset + i] = short.MinValue;
                    }
                    if (gap.size == value.size)
                    {
                        gaps.RemoveAt(gapIndex);
                    }
                    else
                    {
                        gaps[gapIndex] = (gap.offset + value.size, gap.size - value.size);
                    }
                    break;
                }
            }
        }
        long sum = data.Select<short, long>((val, index) => val == short.MinValue ? (long)0 : (long)val * (long)index).Sum();
        return sum;
    }
}