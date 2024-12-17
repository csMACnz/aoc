using System.Diagnostics;

public static class Processor
{
    public static (long A, long B, long C, ICollection<byte> output) Run(string[] sample)
    {
        var (A, B, C, instructions) = Parse(sample);
        return Run(A, B, C, instructions);
    }

    public static (long A, long B, long C, List<byte> instructions) Parse(string[] sample)
    {
        var A = int.Parse(sample[0][12..]);
        var B = int.Parse(sample[1][12..]);
        var C = int.Parse(sample[2][12..]);
        var instructions = sample[4][9..].Split(",").Select(byte.Parse).ToList();
        return (A, B, C, instructions);
    }

    public static long? Part2(List<byte> instructions, int length, long input)
    {
        if (length == instructions.Count) return input;
        var expectedOutput = new List<byte>(instructions)[(instructions.Count - 1 - length)..];
        // Console.Error.WriteLine($"{length}/{instructions.Count}, {input:b}({input})");
        // Console.Error.WriteLine($"I:{string.Join(",", instructions)}");
        // Console.Error.WriteLine($"{string.Join(",", expectedOutput)}");
        {
            foreach (var n in Enumerable.Range(0, 1 << 3))
            {
                var test = (input << 3) | (long)n;
                int indexOffset = 0;
                int outputCount = 0;
                var valid = true;
                long A = test, B = 0, C = 0;
                while (indexOffset < instructions.Count)
                {
                    (A, B, C, indexOffset, var output) = Step(A, B, C, instructions, indexOffset);
                    if (output is not null)
                    {
                        if (outputCount >= expectedOutput.Count || expectedOutput[outputCount] != output)
                        {
                            valid = false;
                            break;
                        }
                        outputCount++;
                    }
                }
                
                if (valid)
                {
                    Console.Error.WriteLine($"O:{string.Join(",", expectedOutput)}({test:b})");
                    var match = Part2(instructions, length + 1, test);
                    if (match.HasValue)
                    {
                        Console.WriteLine(match.Value);
                        return match.Value;
                    }
                }
            }
        }

        return null;
    }

    public static (long A, long B, long C, ICollection<byte> output) Run(long A, long B, long C, List<byte> instructions)
    {
        int indexOffset = 0;
        byte? output = null;
        var outputList = new List<byte>();
        while (indexOffset < instructions.Count)
        {
            (A, B, C, indexOffset, output) = Step(A, B, C, instructions, indexOffset);
            if (output is not null)
            {
                outputList.Add(output.Value);
            }
        }
        return (A, B, C, outputList);
    }

    public static (long A, long B, long C, int nextIndex, byte? output) Step(long A, long B, long C, List<byte> instructions, int indexOffset)
    {
        long comboOperand(byte operand)
        {
            return operand switch
            {
                // Combo operands 0 through 3 represent literal values 0 through 3.
                >= 0 and <= 3 => operand,
                // Combo operand 4 represents the value of register A.
                4 => A,
                // Combo operand 5 represents the value of register B.
                5 => B,
                // Combo operand 6 represents the value of register C.
                6 => C,
                // Combo operand 7 is reserved and will not appear in valid programs.
                7 => throw new UnreachableException("7 is Reserved"),
                _ => throw new UnreachableException("Out of bounds value")
            };
        }

        var operand = instructions[indexOffset + 1];
        return instructions[indexOffset] switch
        {
            // adv instruction (opcode 0) performs division
            0 => (A >> (int)comboOperand(operand), B, C, indexOffset + 2, null),
            // bxl instruction (opcode 1) calculates the bitwise XOR of register B and the instruction's literal operand, then stores the result in register B.
            1 => (A, B ^ operand, C, indexOffset + 2, null),
            // bst instruction (opcode 2) calculates the value of its combo operand modulo 8 (thereby keeping only its lowest 3 bits), then writes that value to the B register.
            2 => (A, comboOperand(operand) % 8, C, indexOffset + 2, null),
            // jnz instruction (opcode 3) does nothing if the A register is 0.
            3 when A is 0 => (A, B, C, indexOffset + 2, null),
            // However, if the A register is not zero, it jumps by setting the instruction pointer to the value of its literal operand; if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
            3 => (A, B, C, operand, null),
            // The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C, then stores the result in register B.
            4 => (A, B ^ C, C, indexOffset + 2, null),
            // The out instruction (opcode 5) calculates the value of its combo operand modulo 8, then outputs that value.
            5 => (A, B, C, indexOffset + 2, (byte)(comboOperand(operand) % 8)),
            // bdv instruction (opcode 6) works exactly like the adv instruction except that the result is stored in the B register.
            6 => (A, A >> (int)comboOperand(operand), C, indexOffset + 2, null),
            // The cdv instruction (opcode 7) works exactly like the adv instruction except that the result is stored in the C register.
            7 => (A, B, A >> (int)comboOperand(operand), indexOffset + 2, null),
            _ => throw new UnreachableException("Out of bounds value")
        };


    }
}
