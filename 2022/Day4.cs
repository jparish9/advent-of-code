using System.Security;

namespace AOC.AOC2022;

public class Day4 : Day<List<Day4.Pairs>>
{
    protected override string? SampleRawInput { get => "2-4,6-8\n2-3,4-5\n5-7,7-9\n2-8,3-7\n6-6,4-6\n2-6,4-8"; }
    // protected override bool Part2ParsedDifferently => true;

    public class Pairs
    {
        public required List<Pair> Values { get; set; }
    }

    public class Pair
    {
        public int Low { get; set; }
        public int High { get; set; }
    }

    protected override long Part1()
    {
        var contained = 0;
        foreach (var line in Input.Select(p => p.Values))
        {
            if ((line[0].Low >= line[1].Low && line[0].High <= line[1].High) ||
                (line[1].Low >= line[0].Low && line[1].High <= line[0].High))
                contained++;
        }

        return contained;
    }

    protected override long Part2()
    {
        var overlap = 0;
        foreach (var line in Input.Select(p => p.Values))
        {
            if ((line[0].Low >= line[1].Low && line[0].Low <= line[1].High) ||
                (line[0].High >= line[1].Low && line[0].High <= line[1].High) ||
                (line[1].Low >= line[0].Low && line[1].Low <= line[0].High) ||
                (line[1].High >= line[0].Low && line[1].High <= line[0].High))
                overlap++;
        }

        return overlap;
    }

    protected override List<Pairs> Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();

        var result = new List<Pairs>();

        foreach (var line in lines)
        {
            var pairsStr = line.Split(',').ToArray();
            var pairs = new List<Pair>();

            foreach (var pair in pairsStr)
            {
                pairs.Add(new () { Low = int.Parse(pair.Split('-')[0]), High = int.Parse(pair.Split('-')[1]) });
            }

            result.Add(new () { Values = pairs });
        }

        return result;
    }
}