// See https://aka.ms/new-console-template for more information
using Dumpify;

Console.WriteLine("Hello, World!");

var line = File.ReadAllText("puzzle.txt");

var data = new List<short>();
short valueIndex = 0;
var onGap = false;
foreach (var ch in line)
{
    var value = short.Parse([ch]);
    foreach (var _ in Enumerable.Range(0, value))
    {
        data.Add(onGap ? short.MinValue : valueIndex);
    }
    if (!onGap)
    {
        valueIndex++;
    }
    onGap = !onGap;
}
long sum = 0;
var readIndex = 0;
while (data.Count != 0)
{
    short value = data[0];
    data.RemoveAt(0);

    if (value == short.MinValue)
    {
        while (value == short.MinValue)
        {
            if (data.Count == 0) break;
            value = data[^1];
            data.RemoveAt(data.Count - 1);
        }
        if (data.Count == 0) break;
    }
    Console.WriteLine($"sum += value({value}) * readIndex({readIndex}) ({value * readIndex})");
    sum += value * readIndex;
    readIndex++;
}

Console.WriteLine($"Part1: {sum}");