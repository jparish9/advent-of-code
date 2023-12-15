namespace AOC.AOC2023;

public class Day11 : Day<Day11.Map>
{
    protected override string? SampleRawInput { get => "...#......\n.......#..\n#.........\n..........\n......#...\n.#........\n.........#\n..........\n.......#..\n#...#....."; }

    public class Map
    {
        public long XSize { get; set; }
        public long YSize { get; set; }
        public required List<Galaxy> Galaxies { get; set; }

        public Map MakeCopy()
        {
            return new Map() { XSize = XSize, YSize = YSize, Galaxies = Galaxies.Select(p => new Galaxy() { X = p.X, Y = p.Y }).ToList() };
        }

        public void Expand(int expansionFactor)
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

        public long SumPairwiseShortestPaths()
        {
            var totalDist = 0L;
            for (var i=0; i<Galaxies.Count; i++)
            {
                for (var j=i+1; j<Galaxies.Count; j++)
                {
                    // Manhattan distance, as there are no obstacles and we can only take cardinal-direction steps
                    totalDist += Math.Abs(Galaxies[i].X - Galaxies[j].X) + Math.Abs(Galaxies[i].Y - Galaxies[j].Y);
                }
            }

            return totalDist;
        }
    }

    public class Galaxy
    {
        public long X { get; set; }
        public long Y { get; set; }
    }

    protected override long Part1()
    {
        // make a copy so we can modify it while still having cached Input between parts
        var map = Input.MakeCopy();
        map.Expand(1);

        return map.SumPairwiseShortestPaths();
    }

    protected override long Part2()
    {
        var map = Input.MakeCopy();
        map.Expand(999999);

        return map.SumPairwiseShortestPaths();
    }

    protected override Map Parse(string input)
    {
        var galaxies = new List<Galaxy>();
        var lines = input.Split('\n').Where(p => p != "").ToArray();

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