using System.Text.RegularExpressions;

namespace AOC.AOC2024;

public partial class Day24 : Day<Day24.Device>
{
    //protected override string? SampleRawInput { get => "x00: 1\nx01: 1\nx02: 1\ny00: 0\ny01: 1\ny02: 0\n\nx00 AND y00 -> z00\nx01 XOR y01 -> z01\nx02 OR y02 -> z02"; }
    // larger example
    protected override string? SampleRawInput { get => "x00: 1\nx01: 0\nx02: 1\nx03: 1\nx04: 0\ny00: 1\ny01: 1\ny02: 1\ny03: 1\ny04: 1\n\nntg XOR fgs -> mjb\ny02 OR x01 -> tnw\nkwq OR kpj -> z05\nx00 OR x03 -> fst\ntgd XOR rvg -> z01\nvdt OR tnw -> bfw\nbfw AND frj -> z10\nffh OR nrd -> bqk\ny00 AND y03 -> djm\ny03 OR y00 -> psh\nbqk OR frj -> z08\ntnw OR fst -> frj\ngnj AND tgd -> z11\nbfw XOR mjb -> z00\nx03 OR x00 -> vdt\ngnj AND wpb -> z02\nx04 AND y00 -> kjc\ndjm OR pbm -> qhw\nnrd AND vdt -> hwm\nkjc AND fst -> rvg\ny04 OR y02 -> fgs\ny01 AND x02 -> pbm\nntg OR kjc -> kwq\npsh XOR fgs -> tgd\nqhw XOR tgd -> z09\npbm OR djm -> kpj\nx03 XOR y03 -> ffh\nx00 XOR y04 -> ntg\nbfw OR bqk -> z06\nnrd XOR fgs -> wpb\nfrj XOR qhw -> z04\nbqk OR frj -> z07\ny03 OR x01 -> nrd\nhwm AND bqk -> z03\ntgd XOR rvg -> z12\ntnw OR pbm -> gnj"; }

    public class Device
    {
        public required Dictionary<string, bool?> Inputs;

        public required List<Instruction> Instructions;

        public long Run()
        {
            var processed = new HashSet<Instruction>();

            while (processed.Count != Instructions.Count)
            {
                // make passes through the instructions, processing those that have all inputs available, until complete
                // could do this as a tree, but this is straightforward enough.
                foreach (var instruction in Instructions)
                {
                    if (processed.Contains(instruction)) continue;

                    var input1 = Inputs[instruction.Input1];
                    var input2 = Inputs[instruction.Input2];

                    if (input1 != null && input2 != null)
                    {
                        // process instruction
                        bool? result = null;

                        switch (instruction.Operation)
                        {
                            case Operation.AND:
                                result = (bool)input1 & (bool)input2!;
                                break;
                            case Operation.OR:
                                result = (bool)input1 | (bool)input2;
                                break;
                            case Operation.XOR:
                                result = (bool)input1 ^ (bool)input2;
                                break;
                        }

                        Inputs[instruction.Output] = result;

                        processed.Add(instruction);
                    }
                }
            }

            // compute result as a base-10 number
            var output = 0L;
            foreach (var kv in Inputs.Where(p => p.Key.StartsWith('z')))
            {
                if (kv.Value == true)
                {
                    output += 1L << int.Parse(kv.Key[1..]);
                }
            }
            return output;
        }

        public List<string> FindInvalid()
        {
            // this is impossible to brute-force as the inputs are very large (in the case of the input, 45-bit) numbers.
            // however, we can consider them bit-by-bit, as our desired result is that x + y = z.
            // if x00 + y00 != z00, then we know that some wire involved in the calculation of z00 needs to be swapped.
            // need to deal with carries though.  if x01 + y01 != z01, then we need to check if the carry from x00 + y00 is involved.

            // I realized this circuit is a chain of adder circuits, but didn't quite get to the answer without consulting some reddit threads.

            // this is basically determining where this circuit is not correct as a ripple-carry adder.
            // a ripple-carry adder (for a fixed number of bits) is:
            // initial step/least significant bit (half-adder):
            //  - output = A XOR B
            //  - carry = A AND B
            // subsequent steps (full adder):
            // - output is A XOR B XOR Cin  (Cin = carry-in from previous step)
            // - carry is (A AND B) OR ((A XOR B) AND Cin)
            // last carry is most significant bit

            // therefore (referencing https://en.wikipedia.org/wiki/File:Fulladder.gif and making exceptions for the inital half-adder and final digit output):
            // - XOR can only output (z*) a bit if it doesn't take an input (x*, y*) bit  (unless operating on initial input)
            // - XOR only takes an input if followed by another XOR and AND (unless operating on the initial input)
            // - AND is only followed by OR (unless operating on initial input)
            // - OR must be followed by both AND and XOR unless outputting final digit (z[max])
            // - any intermediate operation (inputs not x,y and output not z) must not be XOR

            var invalid = new List<string>();
            var finalOutput = "z" + Instructions.Where(p => p.Output.StartsWith('z')).Select(p => int.Parse(p.Output[1..])).Max();

            foreach (var instr in Instructions)
            {
                var next = Instructions.Where(p => p.Input1 == instr.Output || p.Input2 == instr.Output).ToList();

                // check rules

                // XOR can only output (z*) a bit if it doesn't take an input (x*, y*) bit  (except the least significant bit)
                if (instr.Operation == Operation.XOR && !instr.IsInitialInput && instr.ProducesPrimaryOutput && instr.TakesPrimaryInput)
                {
                    invalid.Add(instr.Output);
                }
                // XOR only takes an input if followed by another XOR and AND (unless it's a carry from initial half adder)
                else if (instr.Operation == Operation.XOR
                    && !instr.IsInitialInput && instr.TakesPrimaryInput
                    && (next.Count(p => p.Operation == Operation.XOR) != 1 || next.Count(p => p.Operation == Operation.AND) != 1))
                {
                    invalid.Add(instr.Output);
                }
                // AND is only followed by OR, unless operating on x00 and y00 (initial half adder carry)
                else if (instr.Operation == Operation.AND && !instr.IsInitialInput && next.Any(p => p.Operation != Operation.OR))
                {
                    invalid.Add(instr.Output);
                }
                // OR must be followed by both AND and XOR, but not OR, unless outputting final digit (z[max])
                else if (instr.Operation == Operation.OR && instr.Output != finalOutput
                    && (next.Count(p => p.Operation == Operation.AND) != 1 || next.Count(p => p.Operation == Operation.XOR) != 1))
                {
                    invalid.Add(instr.Output);
                }
                // any intermediate operation (inputs not x,y and output not z) must not be XOR
                else if (instr.Operation == Operation.XOR && !instr.TakesPrimaryInput && !instr.ProducesPrimaryOutput)
                {
                    invalid.Add(instr.Output);
                }
                // if the output is z*, the operation must be XOR unless the output is the final digit (z[max]).
                else if (instr.ProducesPrimaryOutput && instr.Operation != Operation.XOR && instr.Output != finalOutput)
                {
                    invalid.Add(instr.Output);
                }
            }

            return invalid;
        }
    }

    public class Instruction
    {
        public Operation Operation;
        public required string Input1;
        public required string Input2;
        public required string Output;

        public bool TakesPrimaryInput => Input1.StartsWith('x') || Input1.StartsWith('y') || Input2.StartsWith('x') || Input2.StartsWith('y');
        public bool ProducesPrimaryOutput => Output.StartsWith('z');
        public bool IsInitialInput => Input1 == "x00" || Input1 == "y00" || Input2 == "x00" || Input2 == "y00";
    }

    public enum Operation
    {
        AND,
        OR,
        XOR
    }

    protected override Answer Part1()
    {
        return Input.Run();
    }

    protected override Answer Part2()
    {
        if (IsSampleInput) return "ignored";            // the sample is much simplified and isn't a chain of adders

        return string.Join(",", Input.FindInvalid().OrderBy(p => p));
    }

    protected override Device Parse(RawInput input)
    {
        var inputs = new Dictionary<string, bool?>();
        var instructions = new List<Instruction>();

        var lines = input.LineGroups();

        foreach (var line in lines[0])
        {
            var parts = line.Split(": ");
            inputs.Add(parts[0], parts[1] == "1");
        }

        foreach (var line in lines[1])
        {
            var match = InstructionMatch().Matches(line)[0];
            
            var input1 = match.Groups[1].Value;
            var input2 = match.Groups[3].Value;
            var op = match.Groups[2].Value;

            var output = match.Groups[4].Value;

            instructions.Add(new Instruction
            {
                Operation = op switch
                {
                    "AND" => Operation.AND,
                    "OR" => Operation.OR,
                    "XOR" => Operation.XOR,
                    _ => throw new Exception("Invalid operation")
                },
                Input1 = input1,
                Input2 = input2,
                Output = output
            });
        }

        // add null input keys for every output
        foreach (var instruction in instructions)
        {
            if (!inputs.ContainsKey(instruction.Output))
            {
                inputs.Add(instruction.Output, null);
            }
        }

        return new Device() { Inputs = inputs, Instructions = instructions };
    }

    [GeneratedRegex(@"(.*) (AND|OR|XOR) (.*) -> (.*)")]
    private static partial Regex InstructionMatch();

}