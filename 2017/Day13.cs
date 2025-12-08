namespace AOC.AOC2017;

public class Day13 : Day<Day13.Firewall>
{
    protected override string? SampleRawInput { get => "0: 3\n1: 2\n4: 4\n6: 4"; }

    public class Firewall
    {
        public required Dictionary<int, int> Layers { get; set; }
    }

    protected override Answer Part1()
    {
        return TestPacket(0, false).Severity;
    }

    protected override Answer Part2()
    {
        // there might be a more clever way to do this using mod and/or factorization, but brute force is fast enough (~2s)
        var delay = 0;
        while (true)
        {
            if (!TestPacket(delay, true).Caught) break;
            delay++;
        }
        return delay;
    }

    private (bool Caught, int Severity) TestPacket(int delay, bool stopOnCatch)
    {
        var severity = 0;
        var maxLayer = Input.Layers.Keys.Max();
        var caught = false;

        for (var ps = 0; ps <= maxLayer; ps++)
        {
            if (!Input.Layers.ContainsKey(ps)) continue;        // no scanner here

            var range = Input.Layers[ps];
            var cycle = (range - 1) * 2;            // range 2 takes 2 steps to return to the top, 3 takes 4, 4 takes 6, etc.

            if ((ps + delay) % cycle == 0)         // caught
            {
                caught = true;
                if (stopOnCatch) break;
                severity += ps * range;
            }
        }

        return (caught, severity);
    }

    protected override Firewall Parse(RawInput input)
    {
        return new Firewall
        {
            Layers = input.Lines().Select(line =>
            {
                var parts = line.Split(": ");
                return (int.Parse(parts[0]), int.Parse(parts[1]));
            }).ToDictionary(t => t.Item1, t => t.Item2)
        };
    }
}