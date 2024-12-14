// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var machines = new List<Machine>();
for (var i = 0; i < lines.Count(); i += 4)
{
    var buttonA = lines[i][10..].Split(", ").Select(x => int.Parse(x[2..])).ToArray();
    var buttonB = lines[i + 1][10..].Split(", ").Select(x => int.Parse(x[2..])).ToArray();
    var prize = lines[i + 2][7..].Split(", ").Select(x => int.Parse(x[2..])).ToArray();
    var m = new Machine(new Button(buttonA[0], buttonA[1]), new Button(buttonB[0], buttonB[1]), new Prize(prize[0], prize[1]));
    machines.Add(m);
}
var result = 0;
foreach (var machine in machines)
{
    var bCount = machine.Prize.X / machine.B.X;
    var aRemainder = machine.Prize.X % machine.B.X;
    while (bCount >= 0 && aRemainder <= machine.Prize.X && aRemainder % machine.A.X != 0)
    {
        aRemainder += machine.B.X;
        bCount--;
    }
    var additionalACount = aRemainder / machine.A.X;
    var newPrizeX = machine.Prize.X - aRemainder;
    var newPrizeY = machine.Prize.Y - (additionalACount * machine.A.Y);
    var aCount = newPrizeX / machine.A.X;
    var lcm = LCM(machine.A.X, machine.B.X);
    var aDelta = lcm / machine.A.X;
    var bDelta = lcm / machine.B.X;
    while (bCount >= 0 && (machine.A.X * aCount + machine.B.X * bCount != newPrizeX || machine.A.Y * aCount + machine.B.Y * bCount != newPrizeY))
    {
        aCount += aDelta;
        bCount -= bDelta;
    }
    if (bCount < 0)
    {
        Console.WriteLine("Fail: " + aCount + " " + bCount);
        continue;
    }
    Console.WriteLine("partial: " + aCount + " " + bCount);
    result += (aCount +additionalACount) * 3 + bCount;
}

Console.WriteLine("Part1: " + result);

static int GCF(int a, int b)
{
    while (b != 0)
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static int LCM(int a, int b)
{
    return (a / GCF(a, b)) * b;
}

record struct Button(int X, int Y);
record struct Prize(int X, int Y);
record struct Machine(Button A, Button B, Prize Prize);
