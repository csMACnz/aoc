#!/usr/local/share/dotnet/dotnet run
#:package Colorful.Console@1.2.15

Colorful.Console.WriteAscii("AoC 2025 Day 3");

var testInput = """
    987654321111111
    811111111111119
    234234234234278
    818181911112111
    """;

Console.WriteLine($"Expected: (357, 3121910778619)");
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
    var part1 = 0L;
    var part2 = 0L;
    foreach (var bank in banks)
    {
        // // Process each line if needed
        // char firstDigit = bank[0];
        // char? secondDigit = null;
        // for (int i = 1; i < bank.Length; i++)
        // {
        //     if (i < bank.Length - 1 && bank[i] > firstDigit)
        //     {
        //         // not at the end, and current char is greater than current first digit
        //         firstDigit = bank[i];
        //         secondDigit = null;
        //     }
        //     else if (secondDigit.HasValue == false)
        //     {
        //         secondDigit = bank[i];
        //     }
        //     else if (bank[i] > secondDigit.Value)
        //     {
        //         secondDigit = bank[i];
        //     }
        // }
        // var score = (firstDigit - '0') * 10 + (secondDigit.Value - '0');
        var score = ScoreForBank(bank, 2);
        part1 += score;
        part2 += ScoreForBank(bank, 12);
    }

    return (part1, part2);
}

static long ScoreForBank(string bank, int length)
{
    // Process each line if needed
    char?[] digits = new char?[length];
    for (int i = 0; i < bank.Length; i++)
    {
        for (int d = 0; d < length; d++)
        {
            if (digits[d].HasValue == false)
            {
                digits[d] = bank[i];
                if(d+1 < length)
                {
                    //clear the next one, if we are not at the end
                    digits[d+1] = null;
                }
                break;
            }
            else if(i < bank.Length - (length-d-1) && bank[i] > digits[d].Value)
            {
                digits[d] = bank[i];
                if(d + 1 < length){
                    //clear the next one, if we are not at the end
                    digits[d + 1] = null;
                }
                break;
            }
        }
    }
    var score = 0L;
    for (int d = 0; d < length; d++)
    {
        score *= 10;
        score += digits[d].Value - '0';
    }
    return score;
}