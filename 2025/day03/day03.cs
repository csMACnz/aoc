#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

Colorful.Console.WriteAscii("AoC 2025 Day 3");

var testInput = """
    987654321111111
    811111111111119
    234234234234278
    818181911112111
    """;

Console.WriteLine($"Expected: (357, 0)");
Console.WriteLine($" Example: {Puzzle(testInput)}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/3/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
// Console.WriteLine(content);
Console.WriteLine($"   Day03: {Puzzle(content)}");

static (long one, long two) Puzzle(string input)
{
    string[] banks = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var part1 = 0;
    var part2 = 0;
    foreach (var bank in banks)
    {
        // Process each line if needed
        char firstDigit = bank[0];
        char? secondDigit = null;
        for (int i = 1; i < bank.Length; i++)
        {
            if (i < bank.Length - 1 && bank[i] > firstDigit)
            {
                // not at the end, and current char is greater than current first digit
                firstDigit = bank[i];
                secondDigit = null;
            }
            else if (secondDigit.HasValue == false)
            {
                secondDigit = bank[i];
            }
            else if (bank[i] > secondDigit.Value)
            {
                secondDigit = bank[i];
            }
        }
        var score = (firstDigit - '0') * 10 + (secondDigit.Value - '0');
        part1 += score;
    }

    return (part1, part2);
}