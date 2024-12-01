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

    protected override long Part1()
    {
        var diff = 0;
        for (var i=0; i<Input.TotalCount; i++)
        {
            diff += Math.Abs(Input.Left[i] - Input.Right[i]);
        }
        return diff;
    }

    protected override long Part2()
    {
        var sim = 0;
        Input.Left.ForEach(p => sim += p * Input.Right.Count(q => q == p));
        return sim;
    }

    protected override Lists Parse(string input)
    {
        var raw = input.Split('\n').Where(p => p != "").Select(p => p.Split("   ").Select(int.Parse).ToList()).ToList();

        return new Lists() { Left = raw.Select(p => p.First()).OrderBy(p => p).ToList(), Right = raw.Select(p => p.Last()).OrderBy(p => p).ToList(), TotalCount = raw.Count };
    }
}