namespace AOC.AOC2019;

public class Day1 : Day<Day1.Modules>
{
    protected override string? SampleRawInput { get => "12\n14\n1969\n100756"; }

    public class Modules
    {
        public required List<Mass> Masses;
    }

    public class Mass()
    {
        public required int Value;

        public int Fuel(bool recursive = false)
        {
            var fuel = Reduce(Value);
            if (!recursive) return fuel;

            if (fuel <= 0) return 0;
            return fuel + Fuel(fuel);
        }

        private static int Fuel(int mass)
        {
            var fuel = Reduce(mass);
            if (fuel <= 0) return 0;
            return fuel + Fuel(fuel);
        }

        private static int Reduce(int val)
        {
            return val / 3 - 2;
        }
    }

    protected override Answer Part1()
    {
        return Input.Masses.Sum(p => p.Fuel());
    }

    protected override Answer Part2()
    {
        return Input.Masses.Sum(p => p.Fuel(true));
    }

    protected override Modules Parse(RawInput input)
    {
        return new Modules() { Masses = input.Lines().Select(p => new Mass() { Value = int.Parse(p) } ).ToList() };
    }
}