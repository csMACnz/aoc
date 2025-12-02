var testInput = @"L68
L30
R48
L5
R60
L55
L1
L99
R14
L82";
Console.WriteLine($"Day01: {Day01(testInput.Split(Environment.NewLine))}");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=" + Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("AOC_SESSION not set"));
var response = await client.GetAsync("https://adventofcode.com/2025/day/1/input");
var content = (await response.Content.ReadAsStringAsync()).Trim();
// Console.WriteLine(content);
Console.WriteLine($"Day01: {Day01(content.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))}");

static (int one, int two) Day01(IEnumerable<string> input)
{
    var returnsToZeroCount = 0; 
    var passesZeroCount = 0;
    var position = 50;
    foreach (var line in input)
    {
        // Console.WriteLine("START:" + line + ":" + position);
        var startedAt = position;
        var distance = int.Parse(line[1..]);
        if(line[0] == 'L')
        {
            distance = -distance;
        }
        position += distance;

        if(position <= 0 || position >= 100)
        {
            if(position < 0  && startedAt == 0)
            {
                passesZeroCount--;
            }
            while(position < 0)
            {
                // Console.WriteLine("ROTATING:" + line + ":" + position);
                position += 100;
                passesZeroCount++;
            }
            while(position > 100)
            {
                // Console.WriteLine("ROTATING:" + line + ":" + position);
                position -= 100;
                passesZeroCount++;
            }
            if(position == 100 || position == 0)
            {
                if(distance != 0)
                {
                    // Console.WriteLine("ENDDED:" + line + ":" + position);
                    passesZeroCount++;
                }
                position = 0;
            }
        }
        if(position == 0)
        {
            // Console.WriteLine("RESETTING:" + line + ":" + position);
            returnsToZeroCount++;
        }
        // Console.WriteLine("END:" + line + ":" + position);
    }
    return (returnsToZeroCount, passesZeroCount);
}