// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var part1Count = 0;
var part2Count = 0;
foreach (var line in File.ReadAllLines("puzzle.txt"))
{
    part1Count += 2; // wrapping quotes ""-> (-2)
    part2Count += 4; // escaping wrapping quotes ""->"\"\"" (+4)
    var index = 1;
    while (index < line.Length - 1)
    {
        var ch = line[index];
        if (ch != '\\')
        {
            index++;
            continue;
        }
        if (line[index + 1] is '\\' or '"')
        {
            part1Count += 1; // \\=>\ OR \"->" (-1)
            part2Count += 2; // \\=>\\\\ OR \"->\\\" (+2)
            index += 2;
            continue;
        }
        if (line[index + 1] is 'x')
        {
            part1Count += 3; // \x27=>' (-3)
            part2Count += 1; // \x27=>\\x27 (+1)
            index += 4;
            continue;
        }
    }
}

Console.WriteLine("Part1: " + part1Count);
Console.WriteLine("Part2: " + part2Count);