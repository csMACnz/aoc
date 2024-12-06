namespace Day05;

public class SortOptimised
{
    public int Part1(string fileName) { return Run(fileName, false); }
    public int Part2(string fileName) { return Run(fileName, true); }

    private static int Run(string fileName, bool part2)
    {
        var result = 0;
        using StreamReader r = new("puzzle.txt");
        char[] buffer = new char[128];
        int read;
        var rules = new Dictionary<(int left, int right), int>();
        while (r.Peek() is not '\r' or '\n' && (read = r.ReadBlock(buffer, 0, 7)) > 0) // 7 == dd|dd\r\n
        {
            var left = int.Parse(buffer.AsSpan(0..2));
            var right = int.Parse(buffer.AsSpan(3..5));
            rules[(left, right)] = -1;
            rules[(right, left)] = 1;
        }
        if (r.Peek() == '\r')
        {
            r.Read(); // \r
        }
        if (r.Peek() != '\n')
        {
            throw new Exception("Didn't find expected Line Break");
        }
        r.Read(); // \n

        List<int> line = [];
        int number = 0;
        while ((read = r.ReadBlock(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                var (pushNumber, processLine) = buffer[i] switch
                {
                    ',' => (true, false),
                    '\r' => (false, false),
                    '\n' => (true, true),
                    var d when d >= '0' && d <= '9' => (number = (number * 10) + (int)(d - '0'), (false, false)).Item2,
                    var c => throw new Exception($"UnExpected char '{c}'")
                };
                if (pushNumber)
                {
                    line.Add(number);
                    number = 0;
                }
                if (processLine)
                {
                    result += ProcessLine(rules, line, part2) ?? 0;
                    line.Clear();
                }
            }
        }
        
        // finish processing the last line
        line.Add(number);
        result += ProcessLine(rules, line, part2) ?? 0;
        return result;
    }

    private static int? ProcessLine(Dictionary<(int left, int right), int> rules, List<int> line, bool part2)
    {

        var valid = true;
        foreach (var (index, leftNum) in line.Index())
        {
            foreach (int rightNum in line.Skip(index + 1))
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
                line.Sort((l, r) => rules[(l, r)]);
                return line[(line.Count - 1) / 2];
            }
        }
        else
        {
            if (valid)
            {
                return line[(line.Count - 1) / 2];
            }
        }
        return null;
    }
}