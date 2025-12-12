namespace AOC.AOC2025;

public class Day11 : Day<Day11.Network>
{
    protected override string? SampleRawInput { get => "aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out"; }
    protected override string? SampleRawInputPart2 { get => "svr: aaa bbb\naaa: fft\nfft: ccc\nbbb: tty\ntty: ccc\nccc: ddd eee\nddd: hub\nhub: fff\neee: dac\ndac: fff\nfff: ggg hhh\nggg: out\nhhh: out"; }

    public class Network
    {
        public required Dictionary<string, List<string>> Nodes { get; set; }

        public Dictionary<(string from, string to), long> Memoized { get; set; } = [];
    }

    protected override Answer Part1()
    {
        return PathCount("you", "out");
    }

    protected override Answer Part2()
    {
        // valid paths are srv -> ... -> dac -> ... -> fft -> ... -> out
        //               or srv -> ... -> fft -> ... -> dac -> ... -> out
        return PathCount("svr", "dac") * PathCount("dac", "fft") * PathCount("fft", "out")
            + PathCount("svr", "fft") * PathCount("fft", "dac") * PathCount("dac", "out");
    }

    private long PathCount(string from, string to)
    {
        if (Input.Memoized.TryGetValue((from, to), out var cached)) return cached;

        if (from == to) return 1;

        long totalPaths = 0;
        foreach (var neighbor in Input.Nodes[from])
        {
            totalPaths += PathCount(neighbor, to);
        }

        Input.Memoized[(from, to)] = totalPaths;
        return totalPaths;
    }

    protected override Network Parse(RawInput input)
    {
        var nodes = input.Lines().Select(line =>
            {
                var parts = line.Split(": ");
                var node = parts[0];
                var connections = parts[1].Split(' ').ToList();
                return (node, connections);
            }).ToDictionary(p => p.node, p => p.connections);

        nodes.Add("out", []);   // add empty 'out' node

        return new Network
        {
            Nodes = nodes
        };
    }
}