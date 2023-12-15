namespace AOC.AOC2021;

public class Day6 : Day<List<long>>
{
    protected override string? SampleRawInput { get => "3,4,3,1,2"; }

    protected override long Part1()
    {
        return GetPopulation(Input.ToList(), 80);
    }

    protected override long Part2()
    {
        return GetPopulation(Input.ToList(), 256);
    }

    private static long GetPopulation(List<long> data, int days)
    {
        for (var i=0; i<days; i++)
        {
            var zero = data[0];
            for (var j=0; j<8; j++)
            {
                data[j] = data[j+1]; 
            }
            data[8] = zero;
            data[6] += zero;
        }

        return data.Sum(p => p);
    }

    protected override List<long> Parse(string input)
    {
        var binned = new List<long>();
        binned.AddRange(Enumerable.Repeat(0L, 9));

        var nums = input.Split(',').Select(int.Parse).ToList();
        foreach (var num in nums)
        {
            binned[num]++;
        }

        return binned;
    }
}