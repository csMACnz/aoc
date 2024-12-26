// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var part1 = File.ReadAllText("puzzle.txt").Select(ch => ch is '(' ? 1 : -1).Sum();

Console.WriteLine($"Part1: {part1}");
var part2 = File.ReadAllText("puzzle.txt").Select(ch => ch is '(' ? 1 : -1)
    .Aggregate((index: 1, curr: 0, result: -1), (acc, next) => acc.result == -1 ? (acc.curr + next is -1 ? (0, 0, acc.index) : (acc.index + 1, acc.curr + next, -1)) : acc).result;

Console.WriteLine($"Part2: {part2}");