namespace AOC.AOC2021;

public class Day5 : Day<List<Day5.Line>>
{
    protected override string? SampleRawInput { get => "0,9 -> 5,9\n8,0 -> 0,8\n9,4 -> 3,4\n2,2 -> 2,1\n7,0 -> 7,4\n6,4 -> 2,0\n0,9 -> 2,9\n3,4 -> 1,4\n0,0 -> 8,8\n5,5 -> 8,2"; }

    private Dictionary<int, int[]> _result = new();

    public class Line
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

    }

    protected override long Part1()
    {
        return GetCoveredCount()[0];
    }

    protected override long Part2()
    {
        return GetCoveredCount()[1];
    }

    // both parts; cache the result
    private int[] GetCoveredCount()
    {
        if (_result.ContainsKey(InputHashCode)) return _result[InputHashCode];

        var covered = new Dictionary<(int X, int Y, bool isCovered), int>();

        foreach (var line in Input)
        {
            var isDiagonal = line.X1 != line.X2 && line.Y1 != line.Y2;

            var x = line.X1;
            var y = line.Y1;

            while (x != line.X2 || y != line.Y2)
            {
                CountCovered(covered, x, y, isDiagonal);

                x += line.X1 != line.X2 ? (line.X2-line.X1)/Math.Abs(line.X2-line.X1) : 0;
                y += line.Y1 != line.Y2 ? (line.Y2-line.Y1)/Math.Abs(line.Y2-line.Y1) : 0;
            }

            // add the endpoint
            CountCovered(covered, x, y, isDiagonal);
        }

        _result.Add(InputHashCode, new int[] { covered.Count(p => !p.Key.isCovered && p.Value > 1), covered.Count(p => p.Key.isCovered && p.Value > 1) });
        return _result[InputHashCode];
    }

    private static void CountCovered(Dictionary<(int, int, bool), int> covered, int x, int y, bool isDiagonal)
    {
        if (!covered.ContainsKey((x, y, true))) covered.Add((x, y, true), 1);
        else covered[(x, y, true)]++;

        if (!isDiagonal)
        {
            if (!covered.ContainsKey((x, y, false))) covered.Add((x, y, false), 1);
            else covered[(x, y, false)]++;
        }
    }

    protected override List<Line> Parse(string input)
    {
        var lines = new List<Line>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split("->");
            var p1 = parts[0].Split(",");
            var p2 = parts[1].Split(",");

            lines.Add(new Line()
            {
                X1 = int.Parse(p1[0]),
                Y1 = int.Parse(p1[1]),
                X2 = int.Parse(p2[0]),
                Y2 = int.Parse(p2[1])
            });
        }

        return lines;
    }
}