namespace AOC.AOC2024;

public class Day17 : Day<Day17.Program>
{
    protected override string? SampleRawInput { get => "Register A: 729\nRegister B: 0\nRegister C: 0\n\nProgram: 0,1,5,4,3,0"; }
    protected override string? SampleRawInputPart2 { get => "Register A: 2024\nRegister B: 0\nRegister C: 0\n\nProgram: 0,3,5,4,3,0"; }

    public class Program
    {
        public required List<long> Registers;           // [A, B, C]
        public required List<int> Instructions;

        public List<int> Run()
        {
            var pointer = 0;
            var output = new List<int>();
            while (pointer < Instructions.Count)
            {
                var inc = true;
                var instruction = Instructions[pointer];
                var literal = Instructions[pointer+1];
                var combo = literal <= 3 ? literal : literal < 7 ? Registers[literal-4] : -1;
                if (combo == -1) throw new Exception("Invalid combo operand");

                switch (instruction)
                {
                    case 0:     // adv (division), floor(A / 2^(combo operand)) ==> A.  dividing by 2^something is the same as a right bitshift by something, and is much faster.
                        Registers[0] >>= (int)combo;
                        break;
                    case 1:     // bxl (bitwise XOR), B ^ (literal operand) ==> B
                        Registers[1] ^= literal;
                        break;
                    case 2:     // bst (mod), (combo operand) % 8 ==> B         (& 7 is the same as % 8, but faster)
                        Registers[1] = combo & 7;
                        break;
                    case 3:     // jnz (jump if not zero), if A != 0, jump to instruction (literal operand), do not increment pointer
                        if (Registers[0] != 0)
                        {
                            pointer = literal;
                            inc = false;
                        }
                        break;
                    case 4:     // bxc (bitwise XOR), B | C ==> B (ignore operand)
                        Registers[1] ^= Registers[2];
                        break;
                    case 5:     // out (output), (combo operand) % 8  (& 7)
                        output.Add((int)combo & 7);
                        break;
                    case 6:     // bdv (division), like adv except store the result in B
                        Registers[1] = Registers[0] >> (int)combo;
                        break;
                    case 7:     // cdv (division), like adv except store the result in C
                        Registers[2] = Registers[0] >> (int)combo;
                        break;
                }

                if (inc) pointer += 2;
            }

            return output;
        }
    }

    protected override Answer Part1()
    {
        return string.Join(",",Input.Run());
    }

    protected override Answer Part2()
    {
        // brute-force is impossible here (gave up after 16+ billion).
        // after some thought, and eventually consulting the subreddit and other solutions for details (credit github.com/derailed-dash), there is a way to work backward.

        // my input is an 8-instruction loop, where the last instruction restarts the loop if A != 0.
        // the first and third instructions of each loop overwrite B and C based on A.
        // therefore, the starting values of B and C for each loop are irrelevant.
        // the only instruction that modifies A shifts it to the right by 3 bits.
        // thus, for each loop we only care about the 3 least significant bits of A.

        // start by finding the value(s) of 3-bit As that produces the final output.
        // then multiply by 8 and test each of the values from the previous step with (0..7) added to them to see if they match the previous output.
        // repeat for the remaining outputs.  there may be more than one final A that works, then we take the smallest.

        // this is in no way a general solution for any input!
        var validA = new List<long>() { 0 };
        for (var j=Input.Instructions.Count-1; j>=0; j--)
        {
            bool found = false;
            var nextValidA = new List<long>();
            foreach (var currentA in validA)
            {
                for (var i=0; i<=7; i++)
                {
                    var testA = currentA * 8 + i;
                    Input.Registers[0] = testA;
                    var output = Input.Run();
                    if (output[0] == Input.Instructions[j])
                    {
                        found = true;
                        nextValidA.Add(testA);
                    }
                }
            }
            if (!found)
            {
                System.Console.WriteLine("Failed to find a valid A-value for instruction " + (Input.Instructions.Count-j));         // should not happen
            }
            validA = nextValidA;
        }

        return validA.Min();
    }

    protected override Program Parse(RawInput input)
    {
        var parts = input.LineGroups();

        var registers = parts[0].Select(p => long.Parse(p.Split(" ")[2])).ToList();
        var instructions = parts[1][0].Split(": ")[1].Split(",").Select(int.Parse).ToList();

        return new Program() { Registers = registers, Instructions = instructions };
    }
}