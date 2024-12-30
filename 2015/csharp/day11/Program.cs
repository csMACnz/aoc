// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var input = File.ReadAllText("puzzle.txt");

Console.WriteLine(input);
var newPassword = input;
do
{
    newPassword = Increment(newPassword);
} while (!IsValid(newPassword));


Console.WriteLine("Part 1: " + newPassword);

do
{
    newPassword = Increment(newPassword);
} while (!IsValid(newPassword));


Console.WriteLine("Part 2: " + newPassword);

static string Increment(string input)
{
    var rotate = input.ToArray();
    var index = rotate.Length - 1;
    while (index >= 0)
    {
        if (rotate[index] != 'z')
        {
            rotate[index] = (char)(rotate[index] + 1);
            break;
        }
        rotate[index] = 'a';
        index--;
        continue;
    }
    return new String(rotate);
}

static bool IsValid(String password)
{
    var hasStraight = false;
    int? pair1Index = null;
    var hasTwoPairs = false;
    foreach (var index in Enumerable.Range(0, password.Length))
    {
        if (password[index] is 'i' or 'o' or 'l')
        {
            return false;
        }
        if (!hasTwoPairs && index < password.Length - 1)
        {
            if (password[index] == password[index + 1])
            {
                if (pair1Index is null)
                {
                    pair1Index = index;
                }
                else
                {
                    if (pair1Index != index - 1)
                    {
                        hasTwoPairs = true;
                    }
                }
            }
        }
        if (!hasStraight && index < password.Length - 3)
        {
            if (password[index] == password[index + 1] - 1 && password[index + 1] == password[index + 2] - 1)
            {
                hasStraight = true;
            }
        }
    }
    return hasStraight && hasTwoPairs;
}