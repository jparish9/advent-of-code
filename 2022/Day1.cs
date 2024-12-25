namespace AOC.AOC2022;

public class Day1 : Day<List<List<int>>>
{
    protected override string? SampleRawInput { get => "1000\n2000\n3000\n\n4000\n\n5000\n6000\n\n7000\n8000\n9000\n\n10000"; }

    protected override Answer Part1()
    {
        return Input.Max(p => p.Sum());
    }

    protected override Answer Part2()
    {
        return Input.Select(p => p.Sum()).OrderByDescending(p => p).Take(3).Sum();
    }

    protected override List<List<int>> Parse(RawInput input)
    {
        return input.LineGroups().Select(p => p.Select(int.Parse).ToList()).ToList();
    }
}