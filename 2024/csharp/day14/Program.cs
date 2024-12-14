// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
// var file = "sample.txt";
var file = "puzzle.txt";
var width = file == "sample.txt" ? 11 : 101;
var height = file == "sample.txt" ? 7 : 103;
var midX = width / 2;
var midY = height / 2;
var iterations = 100;
var lines = File.ReadAllLines(file);
var topLeft = 0L;
var topRight = 0L;
var bottomLeft = 0L;
var bottomRight = 0L;
var robots = new List<Robot>();
foreach (var line in lines)
{
    var robot = Robot.Parse(line);
    robots.Add(robot);

    var finalPosX = robot.Position.X + (robot.Velocity.X * iterations);
    var finalPosY = robot.Position.Y + (robot.Velocity.Y * iterations);
    while (finalPosX < 0)
    {
        finalPosX += 10 * width;
    }
    while (finalPosY < 0)
    {
        finalPosY += 10 * height;
    }
    var finalPos = (X: finalPosX % width, Y: finalPosY % height);

    if (finalPos.X < midX && finalPos.Y < midY)
    {
        topLeft++;
    }
    else if (finalPos.X > midX && finalPos.Y < midY)
    {
        topRight++;
    }
    else if (finalPos.X < midX && finalPos.Y > midY)
    {
        bottomLeft++;
    }
    else if (finalPos.X > midX && finalPos.Y > midY)
    {
        bottomRight++;
    }
}
var result = topLeft * topRight * bottomLeft * bottomRight;
Console.WriteLine("part1: " + result);

// part 2
foreach (var offset in Enumerable.Range(0, 500000))
{
    var grid = new long[height, width];
    foreach (var robot in robots)
    {
        var finalPosX = robot.Position.X + (robot.Velocity.X * offset);
        var finalPosY = robot.Position.Y + (robot.Velocity.Y * offset);
        while (finalPosX < 0)
        {
            finalPosX += 10 * width;
        }
        while (finalPosY < 0)
        {
            finalPosY += 10 * height;
        }
        var finalPos = (X: finalPosX % width, Y: finalPosY % height);

        grid[finalPos.Y, finalPos.X] = 1;
    }
    var current = 0;
    var max = 0;
    foreach (var y in Enumerable.Range(0, height))
    {
        if (grid[y, midX] != 0)
        {
            current++;
        }
        else
        {
            if (current > max)
            {
                max = current;
                current = 0;
            }
        }
    }
    if (max > 10)
    {
        Print(grid);
        Console.WriteLine("Par2: " + offset);
        break;
    }
    // Print(grid);
}

static void Print(long[,] data)
{
    foreach (var y in Enumerable.Range(0, data.GetLength(0)))
    {
        foreach (var x in Enumerable.Range(0, data.GetLength(1)))
        {

            Console.Write(data[y, x] == 0 ? '.' : '0');
        }
        Console.WriteLine();
    }
}
record struct Point(long X, long Y);
record struct Robot(Point Position, Point Velocity)
{
    public static Robot Parse(string line)
    {
        var parts = line.Split(" ");
        var pos = parts[0][2..].Split(",");
        var posX = long.Parse(pos[0]);
        var posY = long.Parse(pos[1]);
        var vel = parts[1][2..].Split(",");
        var velX = long.Parse(vel[0]);
        var velY = long.Parse(vel[1]);
        return new Robot(
            new Point(posX, posY),
            new Point(velX, velY)
        );
    }
}