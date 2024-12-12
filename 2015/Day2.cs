namespace AOC.AOC2015;

public class Day2 : Day<List<Day2.Dimension>>
{
    protected override string? SampleRawInput { get => "2x3x4"; }

    public class Dimension
    {
        public int L { get; set; }
        public int W { get; set; }
        public int H { get; set; }

    }

    protected override Answer Part1()
    {
        return Input.Select(d =>
            2*d.L*d.W + 2 *d.W*d.H + 2*d.H*d.L                      // surface area
            + new[] { d.L * d.W, d.W * d.H, d.H * d.L }.Min()       // smallest side
        ).Sum();
    }

    protected override Answer Part2()
    {
        return Input.Select(d =>
            2 * new[] { d.L + d.W, d.W + d.H, d.H + d.L }.Min()     // smallest perimeter
            + d.L * d.W * d.H                                       // volume
        ).Sum();
    }

    protected override List<Dimension> Parse(string input)
    {
        var dimensions = new List<Dimension>();
        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split("x");
            dimensions.Add(new Dimension() { L = int.Parse(parts[0]), W = int.Parse(parts[1]), H = int.Parse(parts[2]) });
        }

        return dimensions;
    }
}