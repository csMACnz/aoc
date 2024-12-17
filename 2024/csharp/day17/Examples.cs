namespace day17;

[TestClass]
public sealed class Examples
{
    [TestMethod]
    public void RegisterC9WithInput26ProducesRegisterB1()
    {
        // If register C contains 9, the program 2,6 would set register B to 1.
        var result = Processor.Step(0, 0, 9, [2, 6], 0);
        Assert.AreEqual((0, 1, 9, 2, null), result);
    }

    [TestMethod]
    public void RegisterB29WithInput17ProducesRegisterB26()
    {
        // If register B contains 29, the program 1,7 would set register B to 26.
        var result = Processor.Step(0, 29, 0, [1, 7], 0);
        Assert.AreEqual((0, 26, 0, 2, null), result);
    }

    [TestMethod]
    public void RegisterB2024AndC43690WithInput40ProducesRegisterB44354()
    {
        // If register B contains 2024 and register C contains 43690, the program 4,0 would set register B to 44354.
        var result = Processor.Step(0, 2024, 43690, [4, 0], 0);
        Assert.AreEqual((0, 44354, 43690, 2, null), result);
    }

    [TestMethod]
    public void RegisterA10WithInput505154ProducesRegisterB1()
    {
        // If register A contains 10, the program 5,0,5,1,5,4 would output 0,1,2.
        var result = Processor.Run(10, 0, 0, [5, 0, 5, 1, 5, 4]);
        CollectionAssert.AreEquivalent<byte>([0, 1, 2], result.output, EqualityComparer<byte>.Default);
    }

    [TestMethod]
    public void RegisterCWithInput26ProducesRegisterB1()
    {
        // If register A contains 2024, the program 0,1,5,4,3,0 would output 4,2,5,6,7,7,7,7,3,1,0 and leave 0 in register A.
        var result = Processor.Run(2024, 0, 0, [0, 1, 5, 4, 3, 0]);
        CollectionAssert.AreEquivalent<byte>([4, 2, 5, 6, 7, 7, 7, 7, 3, 1, 0], result.output, EqualityComparer<byte>.Default);
        Assert.AreEqual(0, result.A);
    }

    [TestMethod]
    public void Sample()
    {
        // Register A: 729
        // Register B: 0
        // Register C: 0

        // Program: 0,1,5,4,3,0
        var (_, _, _, output) = Processor.Run(729, 0, 0, [0, 1, 5, 4, 3, 0]);
        CollectionAssert.AreEquivalent<byte>([4, 6, 3, 5, 6, 3, 5, 2, 1, 0], output, EqualityComparer<byte>.Default);
    }

    [TestMethod]
    public void PuzzleFromFilePart1()
    {
        var input = File.ReadAllLines("puzzle.txt");
        var (_, _, _, output) = Processor.Run(input);
        Console.Error.WriteLine("part1: " + string.Join(",", output));
        CollectionAssert.AreEquivalent<byte>([1, 3, 5, 1, 7, 2, 5, 1, 6], output, EqualityComparer<byte>.Default);
    }

    [TestMethod]
    public void SamplePart2()
    {
        var (_, _, _, instructions) = Processor.Parse([
            "Register A: 2024",
            "Register B: 0",
            "Register C: 0",
            "",
            "Program: 0,3,5,4,3,0"
            ]);
        var number = Processor.Part2(instructions, 0, 0);

        Console.Error.WriteLine("part2: " + number);
        Assert.AreEqual(117440, number);
    }

    [TestMethod]
    public void PuzzleFromFilePart2()
    {
        // 2,4 B = A % 8
        // 1,3 B = B ^ 3
        // 7,5 C = A >> B
        // 4,7 B = B ^ C
        // 0,3 A = A >> 3
        // 1,5 B = B ^ 5
        // 5,5 Print(B%8)
        // 3,0 = GOTO 0
        var input = File.ReadAllLines("puzzle.txt");
        var (_, _, _, instructions) = Processor.Parse(input);
        var number = Processor.Part2(instructions, 0, 0);

        Console.Error.WriteLine("part2: " + number);
        Assert.AreEqual(236555997372013, number);
    }
}
