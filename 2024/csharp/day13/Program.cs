// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("puzzle.txt");

var machines = new List<Machine>();
for (var i = 0; i < lines.Count(); i += 4)
{
    var buttonA = lines[i][10..].Split(", ").Select(x => long.Parse(x[2..])).ToArray();
    var buttonB = lines[i + 1][10..].Split(", ").Select(x => long.Parse(x[2..])).ToArray();
    var prize = lines[i + 2][7..].Split(", ").Select(x => long.Parse(x[2..])).ToArray();
    var m = new Machine(
        new Button(buttonA[0], buttonA[1]),
        new Button(buttonB[0], buttonB[1]),
        new Prize(10000000000000 + prize[0], 10000000000000 + prize[1]));
    machines.Add(m);
}
var result = 0L;
foreach (var machine in machines)
{
    var x1 = machine.A.X;
    var y1 = machine.A.Y;
    var x2 = machine.B.X;
    var y2 = machine.B.Y;
    var px = machine.Prize.X;
    var py = machine.Prize.Y;
    // https://www.geeksforgeeks.org/program-for-point-of-intersection-of-two-lines/
    // a1x + b1y = c1 => x1 * a + x2 * b = px
    // a2x + b2y = c2 => y1 * a + y2 * b = py
    //determinant = a1 b2 - a2 b1
    var determinant = x1 * y2 - y1 * x2;
    if (determinant != 0)
    {
        // x = (c1b2 - c2b1)/determinant
        var a = ((px * y2) - (py * x2)) / determinant;
        // y = (a1c2 - a2c1)/determinant
        var b = ((x1 * py) - (y1 * px)) / determinant;

        if (a >= 0 && b >= 0 && a * x1 + b * x2 == px && a * y1 + b * y2 == py)
        {
            result += (3 * a) + b;
        }
    }
}

Console.WriteLine("Part2 expected sample: 875318608908");
Console.WriteLine("Part2 expected puzzle: 83551068361379");
Console.WriteLine("Part2 actual: " + result);

record struct Button(long X, long Y);
record struct Prize(long X, long Y);
record struct Machine(Button A, Button B, Prize Prize);
