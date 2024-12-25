namespace AOC.AOC2024;

public class Day13 : Day<Day13.Arcade>
{
    protected override string? SampleRawInput { get => "Button A: X+94, Y+34\nButton B: X+22, Y+67\nPrize: X=8400, Y=5400\n\nButton A: X+26, Y+66\nButton B: X+67, Y+21\nPrize: X=12748, Y=12176\n\nButton A: X+17, Y+86\nButton B: X+84, Y+37\nPrize: X=7870, Y=6450\n\nButton A: X+69, Y+23\nButton B: X+27, Y+71\nPrize: X=18641, Y=10279"; }

    public class Arcade
    {
        public required List<ClawMachine> ClawMachines;

        public long CalculateTokens()
        {
            var tokens = 0L;
            foreach (var machine in ClawMachines)
            {
                // AD - BC
                var det = machine.AButton.X * machine.BButton.Y - machine.AButton.Y * machine.BButton.X;
                if (det == 0) continue;         // no solution

                // x = A^-1 * B, check for integer solution using mod
                var xInv = machine.BButton.Y * machine.Prize.X - machine.BButton.X * machine.Prize.Y;
                if (xInv % det != 0) continue;
                var yInv = machine.AButton.X * machine.Prize.Y - machine.AButton.Y * machine.Prize.X;
                if (yInv % det != 0) continue;
                
                tokens += xInv/det * 3 + yInv/det;
            }
            return tokens;
        }
    }

    public class ClawMachine
    {
        public required (long X, long Y) AButton;
        public required (long X, long Y) BButton;

        public required (long X, long Y) Prize;
    }

    protected override Answer Part1()
    {
        return Input.CalculateTokens();
    }

    protected override Answer Part2()
    {
        foreach (var machine in Input.ClawMachines)
        {
            machine.Prize.X += 10000000000000L;
            machine.Prize.Y += 10000000000000L;
        }

        return Input.CalculateTokens();
    }

    protected override Arcade Parse(RawInput input)
    {
        var clawMachines = new List<ClawMachine>();

        var machines = input.LineGroups();

        foreach (var m in machines)
        {
            clawMachines.Add(new ClawMachine() {
                AButton = ParseLine(m[0], '+'),
                BButton = ParseLine(m[1], '+'),
                Prize = ParseLine(m[2], '=')
            });
        }

        return new Arcade() { ClawMachines = clawMachines };
    }

    private static (long X, long Y) ParseLine(string line, char delim)
    {
        var parsed = line.Split(":")[1].Split(", ").Select(x => long.Parse(x.Split(delim)[1])).ToArray();
        return (parsed[0], parsed[1]);
    }
}