// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day06>();

Console.WriteLine("HashGrid Part1: " + new HashGrid("puzzle.txt").Part1());
Console.WriteLine("HashGrid Part2 (First): " + new HashGrid("puzzle.txt").Part2First());
Console.WriteLine("HashGrid Part2 (Faster): " + new HashGrid("puzzle.txt").Part2Faster());

Console.WriteLine("ArrayGrid Part1: " + new ArrayGrid("puzzle.txt").Part1());
Console.WriteLine("ArrayGrid Part2 (First): " + new ArrayGrid("puzzle.txt").Part2First());
Console.WriteLine("ArrayGrid Part2 (Faster): " + new ArrayGrid("puzzle.txt").Part2Faster());
Console.WriteLine("ArrayGrid Part2 (Paralle): " + new ArrayGrid("puzzle.txt").Part2Parallel());

[MemoryDiagnoser]
public class Day06
{
    [Benchmark]
    public void HashGridPart1()
    {
        new HashGrid("puzzle.txt").Part1();
    }
    [Benchmark]
    public void HashGridPart2First()
    {
        new HashGrid("puzzle.txt").Part2First();
    }
    [Benchmark]
    public void HashGridPart2Faster()
    {
        new HashGrid("puzzle.txt").Part2Faster();
    }
    [Benchmark]
    public void ArrayGridPart1()
    {
        new ArrayGrid("puzzle.txt").Part1();
    }
    [Benchmark]
    public void ArrayGridPart2First()
    {
        new ArrayGrid("puzzle.txt").Part2First();
    }
    [Benchmark]
    public void ArrayGridPart2Faster()
    {
        new ArrayGrid("puzzle.txt").Part2Faster();
    }

    [Benchmark]
    public void ArrayGridPart2Parallel()
    {
        new ArrayGrid("puzzle.txt").Part2Parallel();
    }
}
