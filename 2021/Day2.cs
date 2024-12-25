namespace AOC.AOC2021;

public class Day2 : Day<List<Day2.Instruction>>
{
    protected override string? SampleRawInput { get => "forward 5\ndown 5\nforward 8\nup 3\ndown 8\nforward 2"; }

    public class Instruction
    {
        public required string Direction { get; set; }
        public int Distance { get; set; }
    }

    protected override Answer Part1()
    {
        var x = 0;
        var depth = 0;

        foreach (var instr in Input)
        {
            switch (instr.Direction)
            {
                case "forward":
                    x += instr.Distance;
                    break;
                case "up":
                    depth -= instr.Distance;
                    break;
                case "down":
                    depth += instr.Distance;
                    break;
            }
        }

        return x * depth;
    }

    protected override Answer Part2()
    {
        var x=0;
        var depth=0;
        var aim=0;

        foreach (var instr in Input)
        {
            switch (instr.Direction)
            {
                case "forward":
                    x += instr.Distance;
                    depth += aim*instr.Distance;
                    break;
                case "up":
                    aim -= instr.Distance;
                    break;
                case "down":
                    aim += instr.Distance;
                    break;
            }
        }

        return x * depth;
    }

    protected override List<Instruction> Parse(RawInput input)
    {
        return input.Lines()
            .Select(p => new Instruction() { Direction = p.Split(' ')[0], Distance = int.Parse(p.Split(' ')[1]) }).ToList();
    }
}