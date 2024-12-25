namespace AOC.AOC2015;

public class Day6 : Day<Day6.LightGrid>
{
    protected override string? SampleRawInput { get => "turn on 499,499 through 500,500"; }

    public class LightGrid
    {
        public LightGrid()
        {
            Lights = new int[1000][];
            for (var i=0; i<1000; i++)
            {
                Lights[i] = new int[1000];
            }
        }

        public void TurnOffAllLights()
        {
            for (var i=0; i<Lights.Length; i++)
            {
                for (var j=0; j<Lights[0].Length; j++)
                {
                    Lights[i][j] = 0;
                }
            }
        }

        public void ProcessInstructions(Func<Instruction, int, int> turnOn, Func<Instruction, int, int> turnOff, Func<Instruction, int, int> toggle)
        {
            foreach (var instruction in Instructions)
            {
                for (var x = instruction.Start.X; x <= instruction.End.X; x++)
                {
                    for (var y = instruction.Start.Y; y <= instruction.End.Y; y++)
                    {
                        switch (instruction.Type)
                        {
                            case InstructionType.TurnOn:
                                Lights[x][y] = turnOn(instruction, Lights[x][y]);
                                break;
                            case InstructionType.TurnOff:
                                Lights[x][y] = turnOff(instruction, Lights[x][y]);
                                break;
                            case InstructionType.Toggle:
                                Lights[x][y] = toggle(instruction, Lights[x][y]);
                                break;
                        }
                    }
                }
            }
        }

        public int State()
        {
            return Lights.SelectMany(p => p).Sum();
        }

        public required List<Instruction> Instructions { get; set; } = new();

        public int[][] Lights { get; set; }
    }

    public class Instruction
    {
        public InstructionType Type { get; set; }
        public (int X, int Y) Start { get; set; }
        public (int X, int Y) End { get; set; }
    }

    public enum InstructionType
    {
        TurnOn,
        TurnOff,
        Toggle
    }

    protected override Answer Part1()
    {
        Input.TurnOffAllLights();

        Input.ProcessInstructions(
            (i, v) => 1,
            (i, v) => 0,
            (i, v) => 1-v
        );

        return Input.State();
    }

    protected override Answer Part2()
    {
        Input.TurnOffAllLights();

        Input.ProcessInstructions(
            (i, v) => v+1,
            (i, v) => v > 0 ? v-1 : 0,
            (i, v) => v+2
        );

        return Input.State();
    }

    protected override LightGrid Parse(RawInput input)
    {
        var list = new List<Instruction>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" ");
            var instruction = new Instruction();
            if (parts[0] == "turn")
            {
                instruction.Type = parts[1] == "on" ? InstructionType.TurnOn : InstructionType.TurnOff;
                instruction.Start = (int.Parse(parts[2].Split(",")[0]), int.Parse(parts[2].Split(",")[1]));
                instruction.End = (int.Parse(parts[4].Split(",")[0]), int.Parse(parts[4].Split(",")[1]));
            }
            else
            {
                instruction.Type = InstructionType.Toggle;
                instruction.Start = (int.Parse(parts[1].Split(",")[0]), int.Parse(parts[1].Split(",")[1]));
                instruction.End = (int.Parse(parts[3].Split(",")[0]), int.Parse(parts[3].Split(",")[1]));
            }

            list.Add(instruction);
        }

        return new LightGrid() { Instructions = list };
    }
}