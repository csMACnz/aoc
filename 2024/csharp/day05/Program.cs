// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Day05;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day05Benchmarks>();

Console.WriteLine("part 1: " + new BruteForce().Part1("puzzle.txt"));
Console.WriteLine("part 2: " + new BruteForce().Part2("puzzle.txt"));

Console.WriteLine("part 1: " + new Sort().Part1("puzzle.txt"));
Console.WriteLine("part 2: " + new Sort().Part2("puzzle.txt"));

Console.WriteLine("part 1: " + new SortOptimised().Part1("puzzle.txt"));
Console.WriteLine("part 2: " + new SortOptimised().Part2("puzzle.txt"));

[MemoryDiagnoser]
public class Day05Benchmarks
{
    [Benchmark]
    public void BruteForcePart1()
    {
        new BruteForce().Part1("puzzle.txt");
    }

    [Benchmark]
    public void BruteForcePart2()
    {
        new BruteForce().Part2("puzzle.txt");
    }

    [Benchmark]
    public void SortPart1()
    {
        new Sort().Part1("puzzle.txt");
    }

    [Benchmark]
    public void SortPart2()
    {
        new Sort().Part2("puzzle.txt");
    }

    [Benchmark]
    public void SortSpanOptimisedPart1()
    {
        new SortOptimised().Part1("puzzle.txt");
    }

    [Benchmark]
    public void SortSpanOptimisedPart2()
    {
        new SortOptimised().Part2("puzzle.txt");
    }
}