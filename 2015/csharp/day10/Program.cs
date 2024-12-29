// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

string input = File.ReadAllText("puzzle.txt");

foreach (string sample in new string[] { "1", "11", "21", "1211", "111211" })
{
    Console.WriteLine(sample + " => " + Process(sample));
}

var part1Result = Enumerable.Repeat(0, 40).Aggregate(input, (i, _) => Process(i)).Length;
Console.WriteLine("Part1: " + part1Result);

static string Process(string input)
{
    char? last = null;
    var count = 0;
    StringBuilder sb = new StringBuilder();
    foreach (var ch in input)
    {
        if (last == ch)
        {
            count++;
        }
        else
        {
            if (last is not null)
            {
                sb.Append((char)('0' + count));
                sb.Append(last);

            }

            last = ch;
            count = 1;
        }
    }
    sb.Append((char)('0' + count));
    sb.Append(last);

    return sb.ToString();
}