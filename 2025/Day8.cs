namespace AOC.AOC2025;

public class Day8 : Day<Day8.Playground>
{
    protected override string? SampleRawInput { get => "162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689"; }

    public class Playground
    {
        public required List<JunctionBox> JunctionBoxes { get; set; }

        public List<(JunctionBox BoxA, JunctionBox BoxB, double Distance)> Connections { get; set; } = [];

        // compute all pairwise connection distances and sort them
        public void ComputeConnections()
        {
            if (Connections.Count > 0) return;         // already computed

            for (var i=0; i<JunctionBoxes.Count; i++)
            {
                var boxA = JunctionBoxes[i];
                for (var j=i+1; j<JunctionBoxes.Count; j++)
                {
                    var boxB = JunctionBoxes[j];
                    var dist = EuclideanDistance(boxA.Position, boxB.Position);
                    Connections.Add((boxA, boxB, dist));
                }
            }

            Connections.Sort((a, b) => a.Distance.CompareTo(b.Distance));           // sort low-to-high
        }

        public List<HashSet<JunctionBox>> Circuits { get; set; } = [];          // connected state

        public (JunctionBox BoxA, JunctionBox BoxB) LastConnection;             // save last connection made (for part 2)
    }

    public class JunctionBox
    {
        public required (long x, long y, long z) Position { get; set; }
    }

    protected override Answer Part1()
    {
        ConnectUntil((c, t) => c == (IsSampleInput ? 10 : 1000));
        return Input.Circuits.OrderByDescending(p => p.Count).Take(3).Aggregate(1L, (acc, set) => acc * set.Count);         // multiply sizes of 3 largest circuits
    }

    protected override Answer Part2()
    {
        ConnectUntil((c, t) => t == 1);
        return Input.LastConnection.BoxA.Position.x * Input.LastConnection.BoxB.Position.x;
    }

    private void ConnectUntil(Func<int, int, bool> stopCondition)               // connectionsMade, totalCircuits
    {
        Input.ComputeConnections();

        var connectionsMade = 0;
        var pos = 0;                // position within sorted Connections list

        // every box starts out as its own circuit.
        Input.Circuits = [];
        foreach (var box in Input.JunctionBoxes)
        {
            Input.Circuits.Add([box]);
        }

        // connect until given stop condition
        while (!stopCondition(connectionsMade, Input.Circuits.Count))
        {
            var (boxA, boxB, _) = Input.Connections[pos++];         // next closest connection
            connectionsMade++;

            // find indexes in the circuit list where these boxes are
            var indexA = Input.Circuits.FindIndex((c) => c.Contains(boxA));
            var indexB = Input.Circuits.FindIndex((c) => c.Contains(boxB));

            if (indexA == indexB) continue;         // already part of same circuit

            // merge different circuits
            Input.Circuits[indexA].UnionWith(Input.Circuits[indexB]);
            Input.Circuits.RemoveAt(indexB);

            Input.LastConnection = (boxA, boxB);            // cache for part 2
        }
    }

    private static double EuclideanDistance((long x, long y, long z) a, (long x, long y, long z) b)
    {
        var dx = a.x - b.x;
        var dy = a.y - b.y;
        var dz = a.z - b.z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    protected override Playground Parse(RawInput input)
    {
        return new Playground
        {
            JunctionBoxes = [.. input.Lines().Select(line =>
            {
                var parts = line.Split(',');
                return new JunctionBox { Position = (long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2])) };
            })]
        };
    }
}