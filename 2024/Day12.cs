using System.Data;

namespace AOC.AOC2024;

public class Day12 : Day<Day12.Garden>
{
    protected override string? SampleRawInput { get => "RRRRIICCFF\nRRRRIICCCF\nVVRRRCCFFF\nVVRCCCJFFF\nVVVVCJJCFE\nVVIVCCJJEE\nVVIIICJJEE\nMIIIIIJJEE\nMIIISIJEEE\nMMMISSJEEE"; }

    public class Plot
    {
        public char Type;
        public int? Region;
        public int Y;
        public int X;
        public bool[] CountPerimeters = [true, true, true, true];         // up, right, down, left  (same as GridCardinals)
        public int[] PerimeterSides = [-1, -1, -1, -1];                    // for part 2
    }

    public class Garden
    {
        public required Plot[][] Map;

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < Map[0].Length && y >= 0 && y < Map.Length;
        }

        public void FindRegions()
        {
            var nextRegion = 1;
            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[i].Length; j++)
                {
                    // if this plot doesn't yet have a region yet, start it and flood fill.
                    if (Map[i][j].Region != null) continue;
                    FloodFill(i, j, nextRegion++);
                }
            }
        }

        public long Price()
        {
            var price = 0;
            var regions = Map.SelectMany(p => p).Select(p => p.Region).Distinct();

            foreach (var region in regions)
            {
                var plots = Map.SelectMany(p => p).Where(p => p.Region == region);
                var perimeter = plots.SelectMany(p => p.CountPerimeters).Count(p => p);
                var area = plots.Count();

                price += perimeter*area;
            }

            return price;
        }

        public long BulkPrice()
        {
            var price = 0;
            var regions = Map.SelectMany(p => p).Where(p => p.Region != null).Select(p => (int)p.Region!).Distinct();

            foreach (var region in regions)
            {
                var plots = Map.SelectMany(p => p).Where(p => p.Region == region);
                
                // count connected perimeter in the same direction as a "side"
                var side = 1;
                foreach (var plot in plots)
                {
                    for (var k=0; k<GridCardinals.Length; k++)
                    {
                        var walks = new List<(int x, int y)> { GridCardinals[(k+1)%4], GridCardinals[(k+3)%4] };          // walk left and right if on bottom edge, etc.
                        if (plot.CountPerimeters[k] && plot.PerimeterSides[k] == -1)
                        {
                            plot.PerimeterSides[k] = side;
                            foreach (var walk in walks)
                            {
                                var (x, y) = walk;
                                if (InBounds(plot.X + x, plot.Y + y))
                                {
                                    WalkSide(Map[plot.Y + y][plot.X + x], x, y, side, k, region);
                                }
                            }
                            side++;
                        }
                    }
                }

                var perimeter = plots.SelectMany(p => p.PerimeterSides).Select(p => p).Where(p => p != -1).Distinct().Count();      // count sides
                var area = plots.Count();

                price += perimeter*area;
            }

            return price;
        }

        // DFS flood fill, given a current position and a region id
        private void FloodFill(int i, int j, int region)
        {
            if (Map[i][j].Region != null) return;

            Map[i][j].Region = region;

            // check for connected Region in every direction to nullify mutual perimiters
            for (var k=0; k<GridCardinals.Length; k++)
            {
                var (x, y) = GridCardinals[k];
                if (InBounds(j + x, i + y) && Map[i + y][j + x].Region == region)
                {
                    Map[i][j].CountPerimeters[k] = false;
                    Map[i + y][j + x].CountPerimeters[(k+2)%4] = false;           // opposite direction
                }
            }

            foreach (var (x, y) in GridCardinals)
            {
                if (InBounds(j + x, i + y) && Map[i + y][j + x].Type == Map[i][j].Type)
                {
                    FloodFill(i + y, j + x, region);
                }
            }
        }

        // walk a perimeter "side" in one direction until it ends, marking those edges as part of the same side
        private void WalkSide(Plot p, int xDir, int yDir, int side, int perimiterDirection, int region)
        {
            if (p.Region != region) return;
            if (!p.CountPerimeters[perimiterDirection] || p.PerimeterSides[perimiterDirection] != -1) return;

            p.PerimeterSides[perimiterDirection] = side;

            if (!InBounds(p.X + xDir, p.Y + yDir)) return;

            WalkSide(Map[p.Y+yDir][p.X+xDir], xDir, yDir, side, perimiterDirection, region);
        }
    }

    protected override Answer Part1()
    {
        Input.FindRegions();
        return Input.Price();
    }

    protected override Answer Part2()
    {
        return Input.BulkPrice();
    }

    protected override Garden Parse(RawInput input)
    {
        char[][] map = input.Lines().Select(p => p.ToCharArray()).ToArray();
        Plot[][] plots = new Plot[map.Length][];
        for (int i = 0; i < map.Length; i++)
        {
            plots[i] = new Plot[map[i].Length];
            for (int j = 0; j < map[i].Length; j++)
            {
                plots[i][j] = new Plot() { Type = map[i][j], X = j, Y = i };
            }
        }
        return new Garden() { Map = plots };
    }
}