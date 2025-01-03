namespace AOC.AOC2021;

public class Day7 : Day<List<int>>
{
    protected override string? SampleRawInput { get => "16,1,2,0,4,2,7,1,2,14"; }

    protected override Answer Part1()
    {
        return GetAlignmentCost(p => p);                    // cost is 1 for each step
    }

    protected override Answer Part2()
    {
        return GetAlignmentCost(p => p * (p+1) / 2);        // cost starts at 1, increases by 1 for each step
    }

    private int GetAlignmentCost(Func<int, int> costFunction)
    {
        var minCost = int.MaxValue;
        for (var i=0; i<Input.Max(); i++)
        {
            var cost = Input.Sum(p => costFunction(Math.Abs(p-i)));
            if (cost < minCost)
            {
                minCost = cost;
            }
        }
        return minCost;
    }

    protected override List<int> Parse(RawInput input)
    {
        return input.Value.Split(',').Select(int.Parse).ToList();
    }
}