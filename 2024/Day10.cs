namespace AOC.AOC2024;

public class Day10 : Day<Day10.Map>
{
    protected override string? SampleRawInput { get => "89010123\n78121874\n87430965\n96549874\n45678903\n32019012\n01329801\n10456732"; }

    public class Map
    {
        public required int[][] Grid;

        public List<(int x, int y, List<int> part2Scores)> TrailheadScores = [];         // cached between part 1 and 2; only search grid once

        public void ComputeScores()
        {
            foreach (var h in Trailheads())
            {
                var part2Scores = new List<int>();
                foreach (var end in TrailEnds(h))
                {
                    var reachable = Reachable(h, end);
                    if (reachable > 0)
                        part2Scores.Add(reachable);
                }

                if (part2Scores.Count > 0)
                {
                    TrailheadScores.Add((h.x, h.y, part2Scores));
                }
            }
        }

        private List<(int x, int y)> Trailheads()
        {
            var trailheads = new List<(int x, int y)>();
            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[0].Length; x++)
                {
                    if (Grid[y][x] == 0) trailheads.Add((x, y));
                }
            }
            return trailheads;
        }

        // potentially reachable ends from given trailhead (manhattan distance <= 9)
        private List<(int x, int y)> TrailEnds((int x, int y) trailHead)
        {
            var trailends = new List<(int x, int y)>();
            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[0].Length; x++)
                {
                    if (Grid[y][x] == 9 && Math.Abs(x - trailHead.x) + Math.Abs(y - trailHead.y) <= 9) trailends.Add((x, y));
                }
            }
            return trailends;
        }

        private int Reachable((int x, int y) pos, (int x, int y) end)
        {
            if (pos == end) return 1;

            (int x, int y) = pos;
            var val = Grid[y][x];

            var ct = 0;

            if (x+1 < Grid[0].Length && Grid[y][x+1] == val+1) ct += Reachable((x+1, y), end);
            if (x-1 >= 0 && Grid[y][x-1] == val+1) ct += Reachable((x-1, y), end);
            if (y+1 < Grid.Length && Grid[y+1][x] == val+1) ct += Reachable((x, y+1), end);
            if (y-1 >= 0 && Grid[y-1][x] == val+1) ct += Reachable((x, y-1), end);

            return ct;
        }
    }

    protected override Answer Part1()
    {
        Input.ComputeScores();

        return Input.TrailheadScores.SelectMany(p => p.part2Scores).Count();
    }

    protected override Answer Part2()
    {
        // scores already computed
        return Input.TrailheadScores.SelectMany(p => p.part2Scores).Sum();
    }

    protected override Map Parse(string input)
    {
        return new Map() { Grid = input.Split("\n").Where(p => p != "").Select(l => l.Select(c => c - '0').ToArray()).ToArray() };
    }
}