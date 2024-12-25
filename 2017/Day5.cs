namespace AOC.AOC2017;

public class Day5 : Day<Day5.JumpInstructions>
{
    protected override string? SampleRawInput { get => "0\n3\n0\n1\n-3"; }

    public class JumpInstructions
    {
        public required List<int> Jumps;

        public int CountJumps(bool part2)
        {
            // make a copy
            var cpy = new List<int>(Jumps);

            var ptr = 0;
            var ct = 0;
            while (ptr >= 0 && ptr < cpy.Count)
            {
                var jump = cpy[ptr];
                cpy[ptr] += jump >= 3 && part2 ? -1 : 1;
                ptr += jump;
                ct++;
            }

            return ct;
        }
    }

    protected override Answer Part1()
    {
        return Input.CountJumps(false);
    }

    protected override Answer Part2()
    {
        return Input.CountJumps(true);
    }

    protected override JumpInstructions Parse(RawInput input)
    {
        return new JumpInstructions() { Jumps = input.Lines().Select(int.Parse).ToList() };
    }
}