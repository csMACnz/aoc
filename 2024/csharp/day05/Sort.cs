namespace Day05;

public class Sort
{
    public int Part1(string fileName) { return Run(fileName, false); }
    public int Part2(string fileName) { return Run(fileName, true); }

    private static int Run(string fileName, bool part2)
    {
        var lines = File.ReadAllLines(fileName);

        var count = 0;
        var rules = new Dictionary<(int left, int right), int>();
        while (lines[count] != string.Empty)
        {
            var lineSpan = lines[count].AsSpan();
            var numRangesIter = lineSpan.Split('|');
            numRangesIter.MoveNext();
            var left = int.Parse(lineSpan[numRangesIter.Current]);
            numRangesIter.MoveNext();
            var right = int.Parse(lineSpan[numRangesIter.Current]);
            rules[(left, right)] = -1;
            rules[(right, left)] = 1;
            count++;
        }
        count++;
        var result = 0;
        while (count < lines.Length)
        {
            var valid = true;
            var nums = Parse(lines[count].AsSpan(), ',');
            foreach (var (index, leftNum) in nums.Index())
            {
                foreach (int rightNum in nums.Skip(index+1))
                {
                    if (rules[(leftNum, rightNum)] == 1)
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
                    nums.Sort((l, r) => rules[(l, r)]);
                    result += nums[(nums.Count - 1) / 2];
                }
            }
            else
            {
                if (valid)
                {
                    result += nums[(nums.Count - 1) / 2];
                }
            }
            count++;
        }
        return result;
    }

    private static List<int> Parse(ReadOnlySpan<char> line, char separator)
    {
        List<int> result = [];
        foreach (var numRange in line.Split(separator))
        {
            result.Add(int.Parse(line[numRange]));
        }
        return result;
    }
}