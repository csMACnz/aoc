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
foreach (var line in lines)
{
    var parts = line.Split(" ");
    var pos = parts[0][2..].Split(",");
    var posX = long.Parse(pos[0]);
    var posY = long.Parse(pos[1]);
    var vel = parts[1][2..].Split(",");
    var velX = long.Parse(vel[0]);
    var velY = long.Parse(vel[1]);

    var finalPosX = posX + (velX * iterations);
    var finalPosY = posY + (velY * iterations);
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