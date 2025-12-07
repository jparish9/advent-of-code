namespace AOC.AOC2017;

public class Day8 : Day<Day8.CPU>
{
    protected override string? SampleRawInput { get => "b inc 5 if a > 1\na inc 1 if b < 5\nc dec -10 if a >= 1\nc inc -20 if c == 10"; }

    public class CPU
    {
        public List<Instruction> Instructions { get; set; } = [];

        public Dictionary<int, Cache> Cache { get; set; } = [];
    }

    public class Instruction
    {
        public required string Register { get; set; }
        public required string Operator { get; set; }
        public required int Operand { get; set; }

        public required string ConditionRegister { get; set; }
        public required string ConditionOperator { get; set; }
        public required int ConditionOperand { get; set; }
    }

    public class Cache
    {
        public Dictionary<string, int> Registers { get; set; } = [];
        public int LargestEver { get; set; } = 0;
    }

    protected override Answer Part1()
    {
        RunInstructions();
        return Input.Cache[InputHashCode].Registers.Values.Max();
    }

    protected override Answer Part2()
    {
        RunInstructions();
        return Input.Cache[InputHashCode].LargestEver;
    }

    private void RunInstructions()
    {
        if (Input.Cache.ContainsKey(InputHashCode)) return;         // already run for this input

        var registers = new Dictionary<string, int>();
        var largestEver = 0;
        
        // initialize all registers to 0
        Input.Instructions.Select(p => p.Register).Distinct().ToList().ForEach(r => registers[r] = 0);
        
        foreach (var instr in Input.Instructions)
        {
            var condRegValue = registers[instr.ConditionRegister];

            var conditionMet = instr.ConditionOperator switch
            {
                ">" => condRegValue > instr.ConditionOperand,
                "<" => condRegValue < instr.ConditionOperand,
                ">=" => condRegValue >= instr.ConditionOperand,
                "<=" => condRegValue <= instr.ConditionOperand,
                "==" => condRegValue == instr.ConditionOperand,
                "!=" => condRegValue != instr.ConditionOperand,
                _ => throw new Exception($"Unknown condition operator: {instr.ConditionOperator}"),
            };

            if (conditionMet)
            {
                switch (instr.Operator)
                {
                    case "inc":
                        registers[instr.Register] += instr.Operand;
                        break;
                    case "dec":
                        registers[instr.Register] -= instr.Operand;
                        break;
                    default:
                        throw new Exception($"Unknown operator: {instr.Operator}");
                }

                if (registers[instr.Register] > largestEver)
                {
                    largestEver = registers[instr.Register];
                }
            }
        }

        Input.Cache[InputHashCode] = new Cache() { Registers = registers, LargestEver = largestEver };
    }

    protected override CPU Parse(RawInput input)
    {
        return new CPU
        {
            Instructions = [.. input.Lines().Select(line =>
            {
                var parts = line.Split(' ');

                return new Instruction
                {
                    Register = parts[0],
                    Operator = parts[1],
                    Operand = int.Parse(parts[2]),
                    ConditionRegister = parts[4],
                    ConditionOperator = parts[5],
                    ConditionOperand = int.Parse(parts[6]),
                };
            })]
        };
    }
}