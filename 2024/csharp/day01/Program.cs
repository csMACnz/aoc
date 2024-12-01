// See https://aka.ms/new-console-template for more information
using System.Linq;

Console.WriteLine("Hello, World!");

var lines = File.ReadLines("puzzle.txt");
var leftList = new List<int>();
var rightList = new List<int>();
foreach (var line in lines)
{
    var l = line.Split("   ");
    var a = int.Parse(l[0]);
    var b = int.Parse(l[1]);
    leftList.Add(a);
    rightList.Add(b);
}
leftList.Sort();
rightList.Sort();

var result1 = leftList.Zip(rightList).Select(((int l, int r) x) => Math.Abs(x.l - x.r)).Sum();

Console.WriteLine($"part 1 is: {result1}");

// Part 2
var memo = new Dictionary<int, int>();
var result2 = 0;
foreach (var number in leftList)
{
    if (!memo.ContainsKey(number))
    {
        memo[number] = number * rightList.Count(b => b == number);
    }
    result2 += memo[number];
}

Console.WriteLine($"part 2 is: {result2}");