namespace AOC.AOC2021;

public class Day3 : Day<List<List<bool>>>
{
    protected override string? SampleRawInput { get => "00100\n11110\n10110\n10111\n10101\n01111\n00111\n11100\n10000\n11001\n00010\n01010"; }

    protected override Answer Part1()
    {
        var gamma = 0;
        var epsilon = 0;
        for (var i=0; i<Input[0].Count; i++)
        {
            var total1 = Input.Count(p => p[i]);
            gamma += (1 << Input[0].Count-i-1) * (total1 > Input.Count / 2 ? 1 : 0);
            epsilon += (1 << Input[0].Count-i-1) * (total1 < Input.Count / 2 ? 1 : 0);
        }

        return gamma * epsilon;
    }

    protected override Answer Part2()
    {
        var qualifiedReadings = Input.Select(p => p.ToList()).ToList();
        for (var i=0; i<Input[0].Count; i++)
        {
            var keep = qualifiedReadings.Count(p => p[i]) >= (qualifiedReadings.Count / 2.0);
            
            // remove least common reading
            qualifiedReadings.RemoveAll(p => p[i] != keep);
            if (qualifiedReadings.Count == 1) break;
        }
        var ox = ReadingToInt(qualifiedReadings[0]);

        qualifiedReadings = Input.Select(p => p.ToList()).ToList();
        for (var i=0; i<Input[0].Count; i++)
        {
            var keep = qualifiedReadings.Count(p => p[i]) < (qualifiedReadings.Count / 2.0);
            
            // remove most common reading
            qualifiedReadings.RemoveAll(p => p[i] != keep);
            if (qualifiedReadings.Count == 1) break;
        }

        return ox * ReadingToInt(qualifiedReadings[0]);
    }

    private static int ReadingToInt(List<bool> bools)
    {
        var result = 0;

        for (var i=0; i<bools.Count; i++)
        {
            result += (1 << bools.Count-i-1) * (bools[i] ? 1 : 0);
        }

        return result;
    }

    protected override List<List<bool>> Parse(string input)
    {
        return input.Split('\n').Where(p => p != "").Select(p => p.Select(c => c == '1').ToList()).ToList();
    }
}