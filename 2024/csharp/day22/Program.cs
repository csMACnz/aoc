// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var inputs = File.ReadAllLines("puzzle.txt").Select(long.Parse).ToArray();

var result = 0L;
foreach (var input in inputs)
{
    var end = puzzle(input, 2000);
    Console.WriteLine($"{input}: {end}");
    checked
    {
        result += end;
    }
}
Console.WriteLine("Part1: " + result);

long puzzle(long seed, int iterations)
{
    long result = seed;
    foreach (var iteration in Enumerable.Range(0, iterations))
    {
        result = Psuedo(result);
    }
    return result;
}
long Psuedo(long seed)
{
    var shift = seed << 6; // * 64 == * 2^6 == << 6
    var result = (seed ^ shift) % 16777216;
    shift = result >> 5; // / 32 == / 2^5 == >> 5
    result = (result ^ shift) % 16777216;
    shift = result << 11; // * 2048 == * 2^11 == << 11
    result = (result ^ shift) % 16777216;

    return result;
}