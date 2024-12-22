namespace AOC.AOC2018;

public class Day1 : Day<Day1.Frequency>
{
    protected override string? SampleRawInput { get => "+1\n-2\n+3\n+1"; }

    public class Frequency
    {
        public required List<int> Changes;
    }

    protected override Answer Part1()
    {
        return Input.Changes.Sum();
    }

    protected override Answer Part2()
    {
        var freq = 0;
        var log = new HashSet<int>() { freq };
        int? result = null;
        while (true)
        {
            foreach (var change in Input.Changes)
            {
                freq += change;
                if (log.Contains(freq))
                {
                    result = freq;
                    break;
                }
                log.Add(freq);
            }
            if (result != null) break;
        }

        return result;
    }

    protected override Frequency Parse(string input)
    {
        return new Frequency() { Changes = input.Split("\n").Where(p => p != "").Select(p => int.Parse(p)).ToList() };
    }
}