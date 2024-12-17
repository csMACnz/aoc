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
        CollectionAssert.AreEquivalent([0, 1, 2], result.output, EqualityComparer<int>.Default);
    }

    [TestMethod]
    public void RegisterCWithInput26ProducesRegisterB1()
    {
        // If register A contains 2024, the program 0,1,5,4,3,0 would output 4,2,5,6,7,7,7,7,3,1,0 and leave 0 in register A.
        var result = Processor.Run(2024, 0, 0, [0, 1, 5, 4, 3, 0]);
        CollectionAssert.AreEquivalent([4, 2, 5, 6, 7, 7, 7, 7, 3, 1, 0], result.output, EqualityComparer<int>.Default);
        Assert.AreEqual(0, result.A);
    }

    [TestMethod]
    public void Sample()
    {
        // Register A: 729
        // Register B: 0
        // Register C: 0

        // Program: 0,1,5,4,3,0
        var result = Processor.Run(729, 0, 0, [0, 1, 5, 4, 3, 0]);
        CollectionAssert.AreEquivalent([4, 6, 3, 5, 6, 3, 5, 2, 1, 0], result.output, EqualityComparer<int>.Default);
    }

    [TestMethod]
    public void PuzzleFromFile()
    {
        var input = File.ReadAllLines("puzzle.txt");
        var result = Processor.Run(input);
        Console.WriteLine("part1: " + string.Join(",", result.output));
        CollectionAssert.AreEquivalent([1, 3, 5, 1, 7, 2, 5, 1, 6], result.output, EqualityComparer<int>.Default);
    }
}
