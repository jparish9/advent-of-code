namespace AOC.AOC2023;

public class Day11 : Day<Day11.Map>
{
    protected override string? SampleRawInput { get => "...#......\n.......#..\n#.........\n..........\n......#...\n.#........\n.........#\n..........\n.......#..\n#...#....."; }

    public class Map
    {
        public long XSize { get; set; }
        public long YSize { get; set; }
        public required List<Galaxy> Galaxies { get; set; }

        public long SumPairwiseShortestPaths(int expansionFactor)
        {
            // make a copy as we will be modifying it
            var map = new Map() { XSize = XSize, YSize = YSize, Galaxies = Galaxies.Select(p => new Galaxy() { X = p.X, Y = p.Y }).ToList() };
            map.Expand(expansionFactor);

            var totalDist = 0L;
            for (var i=0; i<map.Galaxies.Count; i++)
            {
                for (var j=i+1; j<map.Galaxies.Count; j++)
                {
                    // Manhattan distance, as there are no obstacles and we can only take cardinal-direction steps
                    totalDist += Math.Abs(map.Galaxies[i].X - map.Galaxies[j].X) + Math.Abs(map.Galaxies[i].Y - map.Galaxies[j].Y);
                }
            }

            return totalDist;
        }
        
        private void Expand(int expansionFactor)
        {
            // if no galaxies found in a given row or column, add expansionFactor empty rows or columns as the next one(s), and skip the expanded rows/colums
            for (var x=0; x<XSize; x++)
            {
                if (!Galaxies.Any(p => p.X == x))
                {
                    Galaxies.Where(p => p.X > x).ToList().ForEach(p => p.X += expansionFactor);
                    x += expansionFactor;
                    XSize += expansionFactor;
                }
            }

            for (var y=0; y<YSize; y++)
            {
                if (!Galaxies.Any(p => p.Y == y))
                {
                    Galaxies.Where(p => p.Y > y).ToList().ForEach(p => p.Y += expansionFactor);
                    y += expansionFactor;
                    YSize += expansionFactor;
                }
            }
        }
    }

    public class Galaxy
    {
        public long X { get; set; }
        public long Y { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.SumPairwiseShortestPaths(1);
    }

    protected override Answer Part2()
    {
        return Input.SumPairwiseShortestPaths(999999);
    }

    protected override Map Parse(RawInput input)
    {
        var galaxies = new List<Galaxy>();
        var lines = input.Lines().ToArray();

        for (var y=0; y<lines.Length; y++)
        {
            for (var x=0; x<lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    galaxies.Add(new Galaxy() { X = x, Y = y });
                }
            }
        }

        return new Map() { Galaxies = galaxies, XSize = lines[0].Length, YSize = lines.Length };
    }
}