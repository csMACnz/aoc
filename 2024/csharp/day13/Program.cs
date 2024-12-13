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
    if (bCount > 100)
    {
        bCount = 100;
        aRemainder = machine.Prize.X - 100 * machine.B.X;
    }
    while (bCount >= 0 && aRemainder <= machine.Prize.X && aRemainder % machine.A.X != 0)
    {
        aRemainder += machine.B.X;
        bCount--;
    }
    var aCount = aRemainder / machine.A.X;
    while (aCount <= 100 && bCount >= 0 && aRemainder <= machine.Prize.X && (machine.A.X * aCount + machine.B.X * bCount != machine.Prize.X || machine.A.Y * aCount + machine.B.Y * bCount != machine.Prize.Y))
    {
        Console.WriteLine(aCount + " " + bCount);
        do
        {
            aRemainder += machine.B.X;
            bCount--;
        } while (bCount >= 0 && aRemainder <= machine.Prize.X && aRemainder % machine.A.X != 0);
        aCount = aRemainder / machine.A.X;
    }
    if (machine.A.X * aCount + machine.B.X * bCount != machine.Prize.X || machine.A.Y * aCount + machine.B.Y * bCount != machine.Prize.Y)
    {
        Console.WriteLine("Fail: " + aCount + " " + bCount);
        continue;
    }
    Console.WriteLine("partial: " + aCount + " " + bCount);
    result += aCount * 3 + bCount;
}

Console.WriteLine("Part1: " + result);
record struct Button(int X, int Y);
record struct Prize(int X, int Y);
record struct Machine(Button A, Button B, Prize Prize);
