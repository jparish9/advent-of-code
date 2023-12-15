namespace AOC.AOC2022;

public class Day1 : Day<List<List<int>>>
{
    protected override string? SampleRawInput { get => "1000\n2000\n3000\n\n4000\n\n5000\n6000\n\n7000\n8000\n9000\n\n10000"; }

    protected override long Part1()
    {
        return Input.Max(p => p.Sum());
    }

    protected override long Part2()
    {
        return Input.Select(p => p.Sum()).OrderByDescending(p => p).Take(3).Sum();
    }

    protected override List<List<int>> Parse(string input)
    {
        var groups = input.Split("\n\n").Where(p => p != "").ToArray();

        var result = new List<List<int>>();

        foreach (var group in groups)
        {
            var lines = group.Split('\n').Where(p => p != "").ToArray();
            var nums = new List<int>();

            foreach (var line in lines)
            {
                nums.Add(int.Parse(line));
            }

            result.Add(nums);
        }

        return result;
    }
}