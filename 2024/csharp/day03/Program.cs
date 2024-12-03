// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");
// new Day3().Part1();
// new Day3().Part2();
// new Day3().Part2Regex();
BenchmarkRunner.Run<Day3>();

[MemoryDiagnoser]
public partial class Day3
{
    [Benchmark]
    public void Part1()
    {
        Run(true);
    }

    [Benchmark]
    public void Part2()
    {
        Run(false);
    }


    [GeneratedRegex(@"(?<term>don't)\(\)|(?<term>do)\(\)|(?<term>mul\((?<num1>\d+),(?<num2>\d+)\))")]
    private static partial Regex MyRegex();

    [Benchmark]
    public void Part2Regex()
    {
        Regex reg = MyRegex();
        var text = File.ReadAllText("puzzle.txt");
        bool parseModeOn = true;
        var result = 0;
        foreach (Match match in reg.Matches(text))
        {
            switch (match.Groups["term"].Value)
            {
                case "don't":
                    parseModeOn = false;
                    break;
                case "do":
                    parseModeOn = true;
                    break;
                default:
                    if (parseModeOn)
                    {
                        result += int.Parse(match.Groups["num1"].Value) * int.Parse(match.Groups["num2"].Value);
                    }
                    break;
            };
        }
        Console.WriteLine($"part2 (regex): {result}");
    }

    private static void Run(bool part1)
    {
        var result = 0;
        var state = State.Do;
        int? a = null, b = null;
        using StreamReader r = new("puzzle.txt");
        char[] buffer = new char[1024];
        int read;
        while ((read = r.ReadBlock(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                (state, a, b) = (state, buffer[i]) switch
                {
                    (State.Do, 'd') => ((State)State.D, (int?)null, (int?)null),
                    (State.D, 'o') => (State.O, null, null),
                    (State.O, 'n') => (State.N, null, null),
                    (State.N, '\'') => (State.Apostrophy, null, null),
                    (State.Apostrophy, 't') => (State.T, null, null),
                    (State.T, '(') => (State.DontLeftParen, null, null),
                    (State.DontLeftParen, ')') => (State.Dont, null, null),
                    (State.Do, 'm') => (State.M, null, null),
                    (State.M, 'u') => (State.U, null, null),
                    (State.U, 'l') => (State.L, null, null),
                    (State.L, '(') => (State.LeftParen, null, null),
                    (State.LeftParen, var digit) when digit >= '0' && digit <= '9' => (State.Num1, int.Parse(digit.ToString()), null),
                    (State.Num1, ',') => (State.Comma, a.Value, null),
                    (State.Num1, var digit) when digit >= '0' && digit <= '9' => (State.Num1, a * 10 + int.Parse(digit.ToString()), null),
                    (State.Comma, var digit) when digit >= '0' && digit <= '9' => (State.Num2, a, int.Parse(digit.ToString())),
                    (State.Num2, ')') => (State.RightParen, a, b.Value),
                    (State.Num2, var digit) when digit >= '0' && digit <= '9' => (State.Num2, a, b * 10 + int.Parse(digit.ToString())),
                    (State.Dont, 'd') => (State.DoD, null, null),
                    (State.Dont, _) => (State.Dont, null, null),
                    (State.DoD, 'o') => (State.DoO, null, null),
                    (State.DoD, _) => (State.Dont, null, null),
                    (State.DoO, '(') => (State.DoLeftParen, null, null),
                    (State.DoO, _) => (State.Dont, null, null),
                    (State.DoLeftParen, ')') => (State.Do, null, null),
                    (State.DoLeftParen, _) => (State.Dont, null, null),
                    (_, _) => (State.Do, null, null)
                };
                if (state == State.RightParen)
                {
                    // Console.WriteLine($"{a.Value}*{b.Value} = {a.Value * b.Value}");
                    result += a.Value * b.Value;
                    a = null;
                    b = null;
                    state = State.Do;
                }
                if (part1 && state == State.Dont)
                {
                    state = State.Do;
                }
            }
        }
        Console.WriteLine($"part {(part1 ? 1 : 2)}: {result}");
    }

    public enum State
    {
        Do,
        M,
        U,
        L,
        LeftParen,
        Num1,
        Comma,
        Num2,
        RightParen,
        D,
        O,
        N,
        Apostrophy,
        T,
        DontLeftParen,
        Dont,
        DoD,
        DoO,
        DoLeftParen,

    }
}