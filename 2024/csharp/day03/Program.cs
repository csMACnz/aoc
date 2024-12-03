// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");
Run(true);
Run(false);

void Run(bool part1)
{

    var lines = File.ReadAllLines("puzzle.txt");

    var result = 0;
    var state = State.Do;
    foreach (var line in lines)
    {
        int? a = null, b = null;
        foreach (var c in line)
        {
            (state, a, b) = (state, c) switch
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