// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hello, World!");
var input = File.ReadAllText("puzzle.txt");

var i = 1;
while (true)
{
    var hash = MD5.HashData(Encoding.ASCII.GetBytes(input + i));
    var test = Convert.ToHexString(hash).Replace("-", "");
    if (test.StartsWith("00000")) { break; }
    checked
    {
        i++;
    }
}
Console.WriteLine($"Part1: " + i);

var j = 1;
while (true)
{
    var hash = MD5.HashData(Encoding.ASCII.GetBytes(input + j));
    var test = Convert.ToHexString(hash).Replace("-", "");
    if (test.StartsWith("000000")) { break; }
    checked
    {
        j++;
    }
}
Console.WriteLine($"Part2: " + j);