// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var niceCountPart1 = 0;
var niceCountPart2 = 0;
foreach (var line in lines)
{
    if (IsNicePart1(line))
    {
        niceCountPart1++;
    }
    if (IsNicePart2(line))
    {
        niceCountPart2++;
    }
}

Console.WriteLine("Part1: " + niceCountPart1);
Console.WriteLine("Part2: " + niceCountPart2);

static bool IsNicePart1(string line)
{
    char? last = null;
    var vowelCount = 0;
    var foundDouble = false;
    foreach (var ch in line)
    {
        if (ch is 'a' or 'e' or 'i' or 'o' or 'u')
        {
            vowelCount++;
        }
        if (last != null)
        {
            if (last == ch)
            {
                foundDouble = true;
            }
            if ((last, ch) is ('a', 'b') or ('c', 'd') or ('p', 'q') or ('x', 'y'))
            {
                return false;
            }
        }
        last = ch;
    }
    return foundDouble && vowelCount >= 3;
}
static bool IsNicePart2(ReadOnlySpan<char> line)
{
    var foundTwoWithGap = false;
    var foundPair = false;
    foreach (var index in Enumerable.Range(1, line.Length - 1))
    {
        char last = line[index - 1];
        char ch = line[index];
        int remainingCount = line.Length - index - 2;
        if (remainingCount > 0)
        {
            foreach (var compIndex in Enumerable.Range(index + 1, remainingCount))
            {
                if (last == line[compIndex] && ch == line[compIndex + 1])
                {
                    foundPair = true;
                    break;
                }
            }
        }

        if (foundPair && foundTwoWithGap) break;

        if (index >= 2)
        {
            char prev = line[index - 2];
            if (prev == ch)
            {
                foundTwoWithGap = true;
                if (foundPair) break;
            }
        }
    }
    return foundTwoWithGap && foundPair;
}