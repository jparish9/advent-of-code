namespace AOC.AOC2024;

public class Day17 : Day<Day17.Program>
{
    protected override string? SampleRawInput { get => "Register A: 729\nRegister B: 0\nRegister C: 0\n\nProgram: 0,1,5,4,3,0"; }
    protected override string? SampleRawInputPart2 { get => "Register A: 2024\nRegister B: 0\nRegister C: 0\n\nProgram: 0,3,5,4,3,0"; }

    private static readonly Dictionary<int, string> OpcodeDescriptions = new Dictionary<int, string>()
    {
        { 0, "adv (A >> OP => A)" },
        { 1, "bxl (B xor OP => B)" },
        { 2, "bst (OP mod 8 => B)" },
        { 3, "jnz (jump to OP if A != 0)" },
        { 4, "bxc (B xor C => B)" },
        { 5, "out (output OP % 8)" },
        { 6, "bdv (A >> OP => B)" },
        { 7, "cdv (A >> OP => C)" }
    };

    public class Program
    {
        public required List<Register> Registers;
        public required List<int> Instructions;

        public string Run()
        {
            var pointer = 0;
            var output = new List<int>();
            //var comboInstructions = new List<int>() { 0, 2, 4, 6, 7 };
            while (pointer < Instructions.Count)
            {
                var inc = true;
                var instruction = Instructions[pointer];
                var literal = Instructions[pointer+1];
                var combo = literal <= 3 ? literal : literal < 7 ? Registers[literal-4].Value : -1;
                //var comboValid = combo >= 0;
                
                /*System.Console.WriteLine("Instruction: " + instruction + " (" + OpcodeDescriptions[instruction] + "), operator " + (comboInstructions.Contains(instruction) ? combo.ToString() : literal.ToString())
                    + ", registers: " + string.Join(",", Registers.Select(p => p.Value)) + ", current output: " + string.Join(",", output));
                */

                /*if (comboInstructions.Contains(instruction) && !comboValid)
                {
                    throw new Exception("Invalid combo value " + literal + ", trying to use with opcode " + instruction);
                }*/

                switch (instruction)
                {
                    case 0:     // adv (division), floor(A / 2^(combo operand)) ==> A.  dividing by 2^something is the same as a right bitshift by something, and is much faster.
                        Registers[0].Value >>= (int)combo;
                        break;
                    case 1:     // bxl (bitwise XOR), B ^ (literal operand) ==> B
                        Registers[1].Value ^= literal;
                        break;
                    case 2:     // bst (mod), (combo operand) % 8 ==> B
                        Registers[1].Value = combo % 8;
                        break;
                    case 3:     // jnz (jump if not zero), if A != 0, jump to instruction (literal operand), do not increment pointer
                        if (Registers[0].Value != 0)
                        {
                            pointer = literal;
                            inc = false;
                        }
                        break;
                    case 4:     // bxc (bitwise XOR), B | C ==> B (ignore operand)
                        Registers[1].Value ^= Registers[2].Value;
                        break;
                    case 5:     // out (output), (combo operand) % 8
                        output.Add((int)combo % 8);
                        break;
                    case 6:     // bdv (division), like adv except store the result in B
                        Registers[1].Value = Registers[0].Value >> (int)combo;
                        break;
                    case 7:     // cdv (division), like adv except store the result in C
                        Registers[2].Value = Registers[0].Value >> (int)combo;
                        break;
                }

                //System.Console.WriteLine("Registers: " + string.Join(",", Registers.Select(p => p.Value)));

                if (inc) pointer += 2;
            }

            return string.Join(",", output);
        }

        public long RunUntil(List<int> TargetOutput, long startA = 0)
        {
            var ARegister = startA;

            var origB = Registers[1].Value;
            var origC = Registers[2].Value;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            while (true)
            {
                var outputPointer = 0;
                Registers[0].Value = ARegister;
                Registers[1].Value = origB;
                Registers[2].Value = origC;

                var pointer = 0;
                var failedOutput = false;

                while (pointer < Instructions.Count && outputPointer < TargetOutput.Count && !failedOutput)
                {
                    var inc = true;
                    var instruction = Instructions[pointer];
                    var literal = Instructions[pointer+1];
                    var combo = literal <= 3 ? literal : literal < 7 ? Registers[literal-4].Value : -1;
                    //var comboValid = combo >= 0;
                    
                    /*System.Console.WriteLine("Instruction: " + instruction + " (" + OpcodeDescriptions[instruction] + "), operator " + (comboInstructions.Contains(instruction) ? combo.ToString() : literal.ToString())
                        + ", registers: " + string.Join(",", Registers.Select(p => p.Value)) + ", current output: " + string.Join(",", output));
                    */

                    /*if (comboInstructions.Contains(instruction) && !comboValid)
                    {
                        throw new Exception("Invalid combo value " + literal + ", trying to use with opcode " + instruction);
                    }*/

                    switch (instruction)
                    {
                        case 0:     // adv (division), floor(A / 2^(combo operand)) ==> A.  dividing by 2^something is the same as a right bitshift by something, and is much faster.
                            Registers[0].Value >>= (int)combo;
                            break;
                        case 1:     // bxl (bitwise XOR), B ^ (literal operand) ==> B
                            Registers[1].Value ^= literal;
                            break;
                        case 2:     // bst (mod), (combo operand) % 8 ==> B
                            Registers[1].Value = combo % 8;
                            break;
                        case 3:     // jnz (jump if not zero), if A != 0, jump to instruction (literal operand), do not increment pointer
                            if (Registers[0].Value != 0)
                            {
                                pointer = literal;
                                inc = false;
                            }
                            break;
                        case 4:     // bxc (bitwise XOR), B | C ==> B (ignore operand)
                            Registers[1].Value ^= Registers[2].Value;
                            break;
                        case 5:     // out (output), (combo operand) % 8
                            if (combo % 8 != TargetOutput[outputPointer]) failedOutput = true;
                            else outputPointer++;
                            break;
                        case 6:     // bdv (division), like adv except store the result in B
                            Registers[1].Value = Registers[0].Value >> (int)combo;
                            break;
                        case 7:     // cdv (division), like adv except store the result in C
                            Registers[2].Value = Registers[0].Value >> (int)combo;
                            break;
                    }

                    if (inc) pointer += 2;
                }

                if (outputPointer == TargetOutput.Count) break;
                ARegister++;

                if (ARegister % 1000000 == 0) {
                    System.Console.WriteLine(ARegister + " in " + sw.ElapsedMilliseconds + " ms");
                }
            }

            return ARegister;
        }
    }

    public class Register
    {
        public required long Value;
    }

    public class Instruction
    {
        public required int OpCode;
        public required int Operand;
    }

    protected override Answer Part1()
    {
        return Input.Run();
    }

    protected override Answer Part2()
    {
        // well, after trying up to 16+ billion, clearly a different approach is needed.
        // there is probably a way to work backward from the output and known instruction set to find the A value(s) that can generate it.

        return Input.RunUntil(Input.Instructions, 2140000000);
    }

    protected override Program Parse(string input)
    {
        var parts = input.Split("\n\n");

        var registers = parts[0].Split("\n").Where(p => p != "").Select(p => new Register() { Value = int.Parse(p.Split(" ")[2]) }).ToList();
        var instructions = parts[1].Split(": ")[1].Split(",").Select(int.Parse).ToList();

        return new Program() { Registers = registers, Instructions = instructions };
    }
}