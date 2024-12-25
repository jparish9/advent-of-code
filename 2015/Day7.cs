namespace AOC.AOC2015;

public class Day7 : Day<Day7.Circuit>
{
    protected override string? SampleRawInput { get => "123 -> x\n456 -> y\nx AND y -> d\nx OR y -> e\nx LSHIFT 2 -> f\ny RSHIFT 2 -> g\nNOT x -> h\nNOT y -> i"; }

    public class Circuit
    {
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
        public Dictionary<string, ushort> Wires { get; set; } = new Dictionary<string, ushort>();

        public void Reset()
        {
            Wires.Clear();
            Instructions.ForEach(p => p.Applied = false);
        }

        public void ApplyInstructions()
        {
            // apply signals first, then iterate others where the inputs are available, until there are no more instructions
            while (Instructions.Any(p => !p.Applied))
            {
                var instr = Instructions.FirstOrDefault(p => !p.Applied && p.CanApply());

                if (instr == null)
                {
                    System.Console.WriteLine(Instructions.Count(p => !p.Applied) + " instructions remain, but none can be applied!");
                    break;
                }

                instr.Apply();
                instr.Applied = true;
            }
        }
    }

    public enum InstructionType { SignalLiteral, SignalWire, And, Or, LShift, RShift, Not }

    public abstract class Instruction
    {
        public required string Output { get; set; }
        public InstructionType Type { get; set; }
        public required Circuit Circuit { get; set; }           // reference to parent circuit
        public bool Applied { get; set; } = false;

        public abstract bool CanApply();
        public abstract void Apply();
    }

    public class SignalInstruction : Instruction
    {
        public ushort Signal { get; set; }

        public override bool CanApply() { return true; }

        public override void Apply()
        {
            if (Circuit.Wires.ContainsKey(Output)) throw new Exception($"Wire '{Output}' already assigned!");
            Circuit.Wires[Output] = Signal;
        }
    }

    public class UnaryInstruction : Instruction
    {
        public required string Input { get; set; }

        public override bool CanApply() { return Circuit.Wires.ContainsKey(Input); }

        public override void Apply()
        {
            if (Circuit.Wires.ContainsKey(Output)) throw new Exception($"Wire '{Output}' already assigned!");
            if (Type == InstructionType.SignalWire) Circuit.Wires[Output] = Circuit.Wires[Input];
            else Circuit.Wires[Output] = (ushort)~Circuit.Wires[Input];         // not
        }
    }

    public class GateInstruction : Instruction
    {
        public ushort? LiteralInput1 { get; set; }
        public string? Input1 { get; set; }
        public required string Input2 { get; set; }

        public override bool CanApply() { return (Input1 == null || Circuit.Wires.ContainsKey(Input1!)) && Circuit.Wires.ContainsKey(Input2); }

        public override void Apply()
        {
            if (Circuit.Wires.ContainsKey(Output)) throw new Exception($"Wire '{Output}' already assigned!");
            var left = LiteralInput1 ?? Circuit.Wires[Input1!];
            if (Type == InstructionType.And) Circuit.Wires[Output] = (ushort)(left & Circuit.Wires[Input2]);
            else Circuit.Wires[Output] = (ushort)(left | Circuit.Wires[Input2]);           // or
        }
    }

    public class ShiftInstruction : Instruction
    {
        public required string Input { get; set; }
        public ushort ShiftBits { get; set; }

        public override bool CanApply() { return Circuit.Wires.ContainsKey(Input); }

        public override void Apply()
        {
            if (Circuit.Wires.ContainsKey(Output)) throw new Exception($"Wire '{Output}' already assigned!");
            if (ShiftBits >= 16) throw new Exception($"Shift bits {ShiftBits} is too large!");
            if (Type == InstructionType.LShift) Circuit.Wires[Output] = (ushort)(Circuit.Wires[Input] << ShiftBits);
            else Circuit.Wires[Output] = (ushort)(Circuit.Wires[Input] >> ShiftBits);           // rshift
        }
    }

    protected override Answer Part1()
    {
        Input.Reset();
        Input.ApplyInstructions();
        return Input.Wires.ContainsKey("a") ? Input.Wires["a"] : -1;
    }

    protected override Answer Part2()
    {
        var part1 = Part1();

        Input.Reset();
        Input.Instructions.RemoveAll(p => p.Output == "b");
        Input.Instructions.Add(new SignalInstruction() { Circuit = Input, Output = "b", Signal = (ushort)part1.Value });
        Input.ApplyInstructions();
        return Input.Wires.ContainsKey("a") ? Input.Wires["a"] : -1;
    }

    protected override Circuit Parse(RawInput input)
    {
        var circuit = new Circuit();
        var list = new List<Instruction>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" -> ");
            var output = parts[1];
            Instruction? i = null;

            if (parts[0].Contains("AND"))
            {
                parts = parts[0].Split(" AND ");
                var isLiteral = ushort.TryParse(parts[0], out var literal);                
                i = new GateInstruction() { Circuit = circuit, Output = output, Type = InstructionType.And, LiteralInput1 = isLiteral ? literal : null, Input1 = isLiteral ? null : parts[0], Input2 = parts[1] };
            }
            else if (parts[0].Contains("OR"))
            {
                parts = parts[0].Split(" OR ");
                i = new GateInstruction() { Circuit = circuit, Output = output, Type = InstructionType.Or, Input1 = parts[0], Input2 = parts[1] };
            }
            else if (parts[0].Contains("LSHIFT"))
            {
                parts = parts[0].Split(" LSHIFT ");
                i = new ShiftInstruction() { Circuit = circuit, Output = output, Type = InstructionType.LShift, Input = parts[0], ShiftBits = ushort.Parse(parts[1]) };
            }
            else if (parts[0].Contains("RSHIFT"))
            {
                parts = parts[0].Split(" RSHIFT ");
                i = new ShiftInstruction() { Circuit = circuit, Output = output, Type = InstructionType.RShift, Input = parts[0], ShiftBits = ushort.Parse(parts[1]) };
            }
            else if (parts[0].Contains("NOT"))
            {
                parts = parts[0].Split("NOT ");
                i = new UnaryInstruction() { Circuit = circuit, Output = output, Type = InstructionType.Not, Input = parts[1] };
            }
            else
            {
                var signal = parts[0];
                if (ushort.TryParse(signal, out var value))
                    i = new SignalInstruction() { Circuit = circuit, Output = output, Type = InstructionType.SignalLiteral, Signal = value };
                else
                    i = new UnaryInstruction() { Circuit = circuit, Output = output, Type = InstructionType.SignalWire, Input = signal };
            }

            list.Add(i);
        }

        circuit.Instructions = list;

        return circuit;
    }
}