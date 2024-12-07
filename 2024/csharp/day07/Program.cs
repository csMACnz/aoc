// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var result = 0L;
foreach (var line in File.ReadAllLines("puzzle.txt"))
{
    var data = line.Split(": ");
    var total = long.Parse(data[0]);
    var arguments = data[1].Split(' ').Select(long.Parse).ToArray();
    bool isValid = IsValid(total, arguments);
    if (isValid)
    {
        result += total;
    }
}
Console.WriteLine(result);

static bool IsValid(decimal total, Span<long> arguments)
{
    if (arguments.Length == 1) return total == arguments[0];
    return IsValid(total / arguments[^1], arguments[..^1]) || IsValid(total - arguments[^1], arguments[..^1]);
}