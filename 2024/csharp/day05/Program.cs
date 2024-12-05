// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");
var part2 = true;
var count = 0;
var rules = new List<(string left, string right)>();
while (lines[count] != string.Empty)
{
    var nums = lines[count].Split('|').ToArray();
    var left = nums[0];
    var right = nums[1];
    rules.Add((left, right));
    count++;
}
count++;
var result = 0;
while (count < lines.Length)
{
    var valid = true;
    var nums = lines[count].Split(',').ToList();
    foreach (var (left, right) in rules)
    {
        if (nums.Contains(left) && nums.Contains(right))
        {
            if (nums.FindIndex(l => l == left) > nums.FindIndex(0, r => r == right))
            {
                valid = false;
                break;
            }
        }
    }
    if (part2)
    {
        if (!valid)
        {
            bool unsorted = true;
            while (unsorted)
            {
                bool valid2 = true;
                foreach (var (left, right) in rules)
                {
                    if (nums.Contains(left) && nums.Contains(right))
                    {
                        var leftIndex = nums.FindIndex(l => l == left);
                        var rightIndex = nums.FindIndex(0, r => r == right);
                        if (leftIndex > rightIndex)
                        {
                            (nums[rightIndex], nums[leftIndex]) = (nums[leftIndex], nums[rightIndex]);
                            valid2 = false;
                            break;
                        }
                    }
                }
                if (valid2)
                {
                    unsorted = false;
                }
            }
            result += int.Parse(nums[(nums.Count - 1) / 2]);
        }
    }
    else
    {
        if (valid)
        {
            result += int.Parse(nums[(nums.Count - 1) / 2]);
        }
    }
    count++;
}
Console.WriteLine(result);