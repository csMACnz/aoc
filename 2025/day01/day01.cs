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

static int Day01(IEnumerable<string> input)
{
    var returnsToZeroCount = 0; 
    var position = 50;
    foreach (var line in input)
    {
        var distance = int.Parse(line[1..]);
        if(line[0] == 'L')
        {
            distance = -distance;
        }
        position = (position + distance + 100) % 100;
        // Console.WriteLine($"{line}: Current position: {position}");
        if(position % 100 == 0)
        {
            returnsToZeroCount++;
        }
    }
    return returnsToZeroCount;
}