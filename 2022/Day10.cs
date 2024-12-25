namespace AOC.AOC2022;

public class Day10 : Day<List<Day10.Instruction>>
{
    protected override string? SampleRawInput { get => "addx 15\naddx -11\naddx 6\naddx -3\naddx 5\naddx -1\naddx -8\naddx 13\naddx 4\nnoop\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx -35\naddx 1\naddx 24\naddx -19\naddx 1\naddx 16\naddx -11\nnoop\nnoop\naddx 21\naddx -15\nnoop\nnoop\naddx -3\naddx 9\naddx 1\naddx -3\naddx 8\naddx 1\naddx 5\nnoop\nnoop\nnoop\nnoop\nnoop\naddx -36\nnoop\naddx 1\naddx 7\nnoop\nnoop\nnoop\naddx 2\naddx 6\nnoop\nnoop\nnoop\nnoop\nnoop\naddx 1\nnoop\nnoop\naddx 7\naddx 1\nnoop\naddx -13\naddx 13\naddx 7\nnoop\naddx 1\naddx -33\nnoop\nnoop\nnoop\naddx 2\nnoop\nnoop\nnoop\naddx 8\nnoop\naddx -1\naddx 2\naddx 1\nnoop\naddx 17\naddx -9\naddx 1\naddx 1\naddx -3\naddx 11\nnoop\nnoop\naddx 1\nnoop\naddx 1\nnoop\nnoop\naddx -13\naddx -19\naddx 1\naddx 3\naddx 26\naddx -30\naddx 12\naddx -1\naddx 3\naddx 1\nnoop\nnoop\nnoop\naddx -9\naddx 18\naddx 1\naddx 2\nnoop\nnoop\naddx 9\nnoop\nnoop\nnoop\naddx -1\naddx 2\naddx -37\naddx 1\naddx 3\nnoop\naddx 15\naddx -21\naddx 22\naddx -6\naddx 1\nnoop\naddx 2\naddx 1\nnoop\naddx -10\nnoop\nnoop\naddx 20\naddx 1\naddx 2\naddx 2\naddx -6\naddx -11\nnoop\nnoop\nnoop"; }

    public class Instruction
    {
        public required string Operation { get; set; }
        public int Cycles { get; set; }
        public int? Argument { get; set; }
    }

    public class CPU
    {
        public int X { get; set; } = 1;
        public int Cycles = 1;

        private Instruction? CurrentInstruction { get; set;}

        public CPU(Queue<Instruction> instructions)
        {
            Instructions = instructions;
        }

        public Queue<Instruction> Instructions { get; set; }

        public bool Step()
        {
            if (CurrentInstruction == null && !Instructions.Any()) return false;

            CurrentInstruction ??= Instructions.Dequeue();

            CurrentInstruction.Cycles--;
            Cycles++;

            if (CurrentInstruction.Cycles == 0)
            {
                if (CurrentInstruction.Operation == "addx")
                {
                    X += CurrentInstruction.Argument!.Value;
                }

                CurrentInstruction = null;
            }

            return true;
        }
    }

    protected override Answer Part1()
    {
        var signalStrength = 0;

        // make copy of instructions since their cycles [remaining] get modified
        var instrs = Input.Select(p => new Instruction() { Operation = p.Operation, Argument = p.Argument, Cycles = p.Cycles }).ToList();
        var cpu = new CPU(new Queue<Instruction>(instrs));

        while (cpu.Step())
        {
            if ((cpu.Cycles+20) % 40 == 0 && cpu.Cycles <= 220)
            {
                signalStrength += cpu.Cycles * cpu.X;
            }
        }

        return signalStrength;
    }

    protected override Answer Part2()
    {
        var raster = new char[6][];
        for (var i=0; i<raster.Length; i++)
        {
            raster[i] = new char[40];
            Array.Fill(raster[i], ' ');
        }

        // make copy of instructions since their cycles [remaining] get modified
        var instrs = Input.Select(p => new Instruction() { Operation = p.Operation, Argument = p.Argument, Cycles = p.Cycles }).ToList();
        var cpu = new CPU(new Queue<Instruction>(instrs));

        do
        {
            if (cpu.Cycles > 240) break;

            // drawing sprite is 3 pixels wide, centered on current cpu.X
            // raster is stepped through each cycle.
            // if drawing sprite is on top of current position in raster, then render #
            if ((cpu.Cycles-1) % 40 >= cpu.X - 1 && ((cpu.Cycles-1) % 40 <= cpu.X + 1))
            {
                raster[(cpu.Cycles-1) / 40][(cpu.Cycles-1) % 40] = '#';
            }
        }
        while (cpu.Step());

        return "\n" + string.Join("\n", raster.Select(p => new string(p)));
    }

    protected override List<Instruction> Parse(RawInput input)
    {
        var instructions = new List<Instruction>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(' ');
            instructions.Add(new Instruction() {
                Operation = parts[0],
                Argument = parts.Length > 1 ? int.Parse(parts[1]) : null,
                Cycles = parts[0] == "addx" ? 2 : 1
            });
        }

        return instructions;
    }
}