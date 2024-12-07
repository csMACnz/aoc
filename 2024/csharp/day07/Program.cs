// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var result = 0L;
foreach (var line in File.ReadAllLines("puzzle.txt"))
{
    var data = line.Split(": ");
    var total = long.Parse(data[0]);
    var arguments = data[1].Split(' ').Select(long.Parse).ToArray();
    bool isValid = Part2IsValid(total, arguments);
    if (isValid)
    {
        result += total;
    }
}
Console.WriteLine(result);

static bool Part1IsValid(decimal total, Span<long> arguments)
{
    if (arguments.Length == 1) return total == arguments[0];
    return Part1IsValid(total / arguments[^1], arguments[..^1]) || Part1IsValid(total - arguments[^1], arguments[..^1]);
}

static bool Part2IsValid(decimal total, Span<long> arguments)
{
    if (total < 0) return false;
    if (total != Math.Truncate(total)) return false;
    if (arguments.Length == 1) return total == arguments[0];
    if (("" + total).Length > ("" + arguments[^1]).Length && ("" + total).EndsWith("" + arguments[^1]) && Part2IsValid(long.Parse(("" + total).Substring(0, ("" + total).Length - ("" + arguments[^1]).Length)), arguments[..^1]))
    {
        return true;
    }
    return Part2IsValid(total / arguments[^1], arguments[..^1]) || Part2IsValid(total - arguments[^1], arguments[..^1]);
}