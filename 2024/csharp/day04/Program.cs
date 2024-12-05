// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Day04;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Day04BenchMarks>();


Console.WriteLine("DictionaryGridSolution:");
var first = new DictionaryGridSolution("puzzle.txt");
Console.WriteLine($"Part1: {first.Part1()}");
Console.WriteLine($"Part2: {first.Part2()}");

Console.WriteLine("DictionaryGridWithXACacheSolution:");
var firstWithCache = new DictionaryGridWithXACacheSolution("puzzle.txt");
Console.WriteLine($"Part1: {firstWithCache.Part1()}");
Console.WriteLine($"Part2: {firstWithCache.Part2()}");

Console.WriteLine("ArrayGridSolution:");
var arrayGrid = new ArrayGridSolution("puzzle.txt");
Console.WriteLine($"Part1: {arrayGrid.Part1()}");
Console.WriteLine($"Part2: {arrayGrid.Part2()}");
Console.WriteLine($"Part2RemovedBoundsChecls: {arrayGrid.Part2RemoveBoundsChecks()}");

[MemoryDiagnoser]
public class Day04BenchMarks()
{
    [Benchmark]
    public void DictionaryGridSolutionPart1()
    {
        var grid = new DictionaryGridSolution("puzzle.txt");
        grid.Part1();
    }

    [Benchmark]
    public void DictionaryGridSolutionPart2()
    {
        var grid = new DictionaryGridSolution("puzzle.txt");
        grid.Part2();
    }

    [Benchmark]
    public void DictionaryGridWithXACacheSolutionPart1()
    {
        var grid = new DictionaryGridWithXACacheSolution("puzzle.txt");
        grid.Part1();
    }

    [Benchmark]
    public void DictionaryGridWithXACacheSolutionPart2()
    {
        var grid = new DictionaryGridWithXACacheSolution("puzzle.txt");
        grid.Part2();
    }

    [Benchmark]
    public void ArrayGridSolution()
    {
        var grid = new ArrayGridSolution("puzzle.txt");
        grid.Part1();
    }

    [Benchmark]
    public void ArrayGridSolutionPart2()
    {
        var grid = new ArrayGridSolution("puzzle.txt");
        grid.Part2();
    }
    
    [Benchmark]
    public void ArrayGridSolutionPart2RemoveBoundsChecks()
    {
        var grid = new ArrayGridSolution("puzzle.txt");
        grid.Part2RemoveBoundsChecks();
    }
}