// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var count = 0;
foreach (var line in lines)
{
    var nums = line.Split(' ').Select(n => int.Parse(n)).ToArray();
    var first = nums[0];
    var matches = nums[1] switch
    {
        var x when x > first => growingCheck(nums),
        var y when y < first => shrinkingCheck(nums),
        _ => false
    };
    if (matches)
    {
        count++;
    }
}

Console.WriteLine($"part 1: {count}");

count = 0;
foreach (var line in lines)
{
    var nums = line.Split(' ').Select(n => int.Parse(n)).ToArray();
    var first = nums[0];
    var matches = nums[1] switch
    {
        var x when x > first => growingCheck(nums),
        var y when y < first => shrinkingCheck(nums),
        _ => false
    };
    if (!matches)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            var newN = nums.Take(i).Concat(nums.Skip(i + 1).Take(nums.Length - (i + 1))).ToArray();
            first = newN[0];
            matches = newN[1] switch
            {
                var x when x > first => growingCheck(newN),
                var y when y < first => shrinkingCheck(newN),
                _ => false
            };
            if (matches)
            {
                break;
            }
        }
    }
    if (matches)
    {
        count++;
    }
}

Console.WriteLine($"part 2: {count}");

bool growingCheck(int[] nums)
{
    for (var i = 0; i < nums.Length - 1; i++)
    {
        var a = nums[i];
        var b = nums[i + 1];
        if (a >= b || b - a > 3) return false;
    }
    return true;
}


bool shrinkingCheck(int[] nums)
{
    for (var i = 0; i < nums.Length - 1; i++)
    {
        var a = nums[i];
        var b = nums[i + 1];
        if (a <= b || a - b > 3) return false;
    }
    return true;
}