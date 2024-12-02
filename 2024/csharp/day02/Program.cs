// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var count = 0;
foreach (var line in lines)
{
    var nums = line.Split(' ').Select(n => int.Parse(n)).ToArray();
    var first = nums[0];
    var matches = growingCheck(nums, []) || shrinkingCheck(nums, []);
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
        var x when x > first => growingCheck(nums, []),
        var y when y < first => shrinkingCheck(nums, []),
        _ => false
    };
    if (!matches)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            var lhs = nums[0..i];
            var rhs = nums[(i + 1)..];

            matches = growingCheck(lhs, rhs) || shrinkingCheck(lhs, rhs);
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

bool growingCheck(Span<int> nums, Span<int> moreNums)
{
    return AllPairs(nums, moreNums, (a, b) => a < b && b - a <= 3);
}


bool shrinkingCheck(Span<int> nums, Span<int> moreNums)
{
    return AllPairs(nums, moreNums, (a, b) => a > b && a - b <= 3);
}

bool AllPairs(Span<int> nums, Span<int> moreNums, Func<int, int, bool> check)
{
    var iter = nums.GetEnumerator();
    int? prev = null;
    if (iter.MoveNext())
    {
        prev = iter.Current;
        while (iter.MoveNext())
        {
            var curr = iter.Current;
            if (!check(prev.Value, curr))
            {
                return false;
            }
            prev = curr;
        }
    }
    iter = moreNums.GetEnumerator();
    if (prev is null)
    {
        iter.MoveNext();
        prev = iter.Current;
    }
    while (iter.MoveNext())
    {
        var curr = iter.Current;
        if (!check(prev.Value, curr))
        {
            return false;
        }
        prev = curr;
    }
    return true;
}