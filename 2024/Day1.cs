namespace AOC.AOC2024;

public class Day1 : Day<Day1.Lists>
{
    protected override string? SampleRawInput { get => "3   4\n4   3\n2   5\n1   3\n3   9\n3   3"; }

    public class Lists
    {
        public required List<int> Left { get; set; }
        public required List<int> Right { get; set; }
        public int TotalCount { get; set; }
    }

    protected override Answer Part1()
    {
        var diff = 0;
        for (var i=0; i<Input.TotalCount; i++)
        {
            diff += Math.Abs(Input.Left[i] - Input.Right[i]);
        }
        return diff;
    }

    protected override Answer Part2()
    {
        return Input.Left.Aggregate(0, (acc, p) => acc + p * Input.Right.Count(q => q == p));
    }

    protected override Lists Parse(RawInput input)
    {
        var raw = input.Lines().Select(p => p.Split("   ").Select(int.Parse).ToList()).ToList();

        return new Lists() { Left = raw.Select(p => p.First()).OrderBy(p => p).ToList(), Right = raw.Select(p => p.Last()).OrderBy(p => p).ToList(), TotalCount = raw.Count };
    }
}