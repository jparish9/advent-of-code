namespace AOC.AOC2017;

public class Day1 : Day<Day1.Checksum>
{
    protected override string? SampleRawInput { get => "1221"; }

    public class Checksum
    {
        public required List<int> Digits;

        public int Compute(bool part2)
        {
            var offset = part2 ? Digits.Count / 2 : 1;
            var sum = 0;
            for (var i = 0; i < Digits.Count; i++)
            {
                if (Digits[i] == Digits[(i + offset) % Digits.Count])
                    sum += Digits[i];
            }

            return sum;
        }
    }

    protected override Answer Part1()
    {
        return Input.Compute(false);
    }

    protected override Answer Part2()
    {
        return Input.Compute(true);
    }

    protected override Checksum Parse(string input)
    {
        return new Checksum() { Digits = input.Select(p => int.Parse(p.ToString())).ToList() };
    }
}