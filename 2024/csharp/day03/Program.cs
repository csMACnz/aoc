// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var result = 0;
foreach (var line in lines)
{
    var state = State.Open;
    int? a = null, b = null;
    foreach (var c in line)
    {
        (state, a, b) = (state, c) switch
        {
            (State.Open, 'm') => ((State)State.M, (int?)null, (int?)null),
            (State.M, 'u') => (State.U, null, null),
            (State.U, 'l') => (State.L, null, null),
            (State.L, '(') => (State.LeftParen, null, null),
            (State.LeftParen, var digit) when digit >= '0' && digit <= '9' => (State.Num1, int.Parse(digit.ToString()), null),
            (State.Num1, ',') => (State.Comma, a.Value, null),
            (State.Num1, var digit) when digit >= '0' && digit <= '9' => (State.Num1, a*10+int.Parse(digit.ToString()), null),
            (State.Comma, var digit) when digit >= '0' && digit <= '9' => (State.Num2, a, int.Parse(digit.ToString())),
            (State.Num2, ')') => (State.RightParen, a, b.Value),
            (State.Num2, var digit) when digit >= '0' && digit <= '9' => (State.Num2, a, b*10+int.Parse(digit.ToString())),
            (_, _) => (State.Open, null, null)
        };
        if(state == State.RightParen){
            Console.WriteLine($"{a.Value}*{b.Value} = {a.Value*b.Value}");
            result += a.Value*b.Value;
            a=null;
            b=null;
            state = State.Open;
        }
    }
}
Console.WriteLine(result);

public enum State
{
    Open,
    M,
    U,
    L,
    LeftParen,
    Num1,
    Comma,
    Num2,
    RightParen,
}