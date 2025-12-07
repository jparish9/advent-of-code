namespace AOC.AOC2025;

public class Day7 : Day<Day7.Manifold>
{
    protected override string? SampleRawInput { get => ".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n..............."; }

    public class Manifold
    {
        public required char[][] Grid { get; set; }

        public Dictionary<(int x, int y), long> Cache { get; set; } = [];       // memoization for part 2

        public int StartX() { return Grid[0].ToList().FindIndex(c => c == 'S'); }
    }

    protected override Answer Part1()
    {
        return CountSplits(Input.StartX(), 0);
    }

    protected override Answer Part2()
    {
        return CountTimelines(Input.StartX(), 0);
    }

    private int CountSplits(int xS, int y)
    {
        var splittersHit = 0;
        var beamXs = new HashSet<int>() { xS };
        while (y < Input.Grid.Length)
        {
            // get x positions of all splitters in this row
            var splitters = Input.Grid[y].Select((c, x) => (c, x)).Where(t => t.c == '^').Select(t => t.x).ToList();
            
            // if there are splitter(s) in this row AND beam(s) hit them, count the splitter only once and split the beam(s)
            foreach (var splitterX in splitters)
            {
                if (beamXs.Contains(splitterX))
                {
                    beamXs.Add(splitterX - 1);
                    beamXs.Add(splitterX + 1);
                    beamXs.Remove(splitterX);

                    splittersHit++;
                }
            }

            y++;
        }
        return splittersHit;
    }

    private long CountTimelines(int x, int y)
    {
        // DFS with memoization

        // go down until we find a splitter or reach the bottom
        var thisY = y;
        while (thisY < Input.Grid.Length && Input.Grid[thisY][x] == '.') thisY++;
        if (thisY >= Input.Grid.Length) return 1;           // reached bottom (completed a timeline)

        if (Input.Cache.ContainsKey((x, thisY))) return Input.Cache[(x, thisY)];        // already computed

        // splitter found
        var totalTimelines = CountTimelines(x - 1, thisY + 1) + CountTimelines(x + 1, thisY + 1);
 
        Input.Cache[(x, thisY)] = totalTimelines;           // memoize
        return totalTimelines;
    }

    protected override Manifold Parse(RawInput input)
    {
        return new Manifold
        {
            Grid = [.. input.Lines().Select(line => line.ToCharArray())]
        };
    }
}