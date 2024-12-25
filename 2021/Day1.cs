namespace AOC.AOC2021;

public class Day1 : Day<List<int>>
{
    protected override string? SampleRawInput { get => "199\n200\n208\n210\n200\n207\n240\n269\n260\n263"; }

    protected override Answer Part1()
    {
        return CountIncreased(1);
    }

    protected override Answer Part2()
    {
        return CountIncreased(3);
    }

    private int CountIncreased(int slidingWindow)
    {
        var increased = 0;

        for (var i=slidingWindow; i<Input.Count; i++)
        {
            if (Input[i] > Input[i-slidingWindow]) increased++;
        }

        return increased;
    }

    protected override List<int> Parse(RawInput input)
    {
        return input.Lines().Select(int.Parse).ToList();
    }
}