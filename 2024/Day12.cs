using System.Data;

namespace AOC.AOC2024;

public class Day12 : Day<Day12.Garden>
{
    protected override string? SampleRawInput { get => "RRRRIICCFF\nRRRRIICCCF\nVVRRRCCFFF\nVVRCCCJFFF\nVVVVCJJCFE\nVVIVCCJJEE\nVVIIICJJEE\nMIIIIIJJEE\nMIIISIJEEE\nMMMISSJEEE"; }

    public class Garden
    {
        public required Plot[][] Map;

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
                    if (plot.CountPerimeters[0] && plot.PerimeterSides[0] == -1)
                    {
                        plot.PerimeterSides[0] = side;
                        if (plot.X > 0)
                        {
                            WalkSide(Map[plot.Y][plot.X-1], -1, 0, side, 0, region);
                        }
                        if (plot.X < Map[plot.Y].Length - 1)
                        {
                            WalkSide(Map[plot.Y][plot.X+1], 1, 0, side, 0, region);
                        }
                        side++;
                    }

                    if (plot.CountPerimeters[1] && plot.PerimeterSides[1] == -1)
                    {
                        plot.PerimeterSides[1] = side;
                        if (plot.Y > 0)
                        {
                            WalkSide(Map[plot.Y-1][plot.X], 0, -1, side, 1, region);
                        }
                        if (plot.Y < Map.Length - 1)
                        {
                            WalkSide(Map[plot.Y+1][plot.X], 0, 1, side, 1, region);
                        }
                        side++;
                    }

                    if (plot.CountPerimeters[2] && plot.PerimeterSides[2] == -1)
                    {
                        plot.PerimeterSides[2] = side;
                        if (plot.X > 0)
                        {
                            WalkSide(Map[plot.Y][plot.X-1], -1, 0, side, 2, region);
                        }
                        if (plot.X < Map[plot.Y].Length - 1)
                        {
                            WalkSide(Map[plot.Y][plot.X+1], 1, 0, side, 2, region);
                        }
                        side++;
                    }

                    if (plot.CountPerimeters[3] && plot.PerimeterSides[3] == -1)
                    {
                        plot.PerimeterSides[3] = side;
                        if (plot.Y > 0)
                        {
                            WalkSide(Map[plot.Y-1][plot.X], 0, -1, side, 3, region);
                        }
                        if (plot.Y < Map.Length - 1)
                        {
                            WalkSide(Map[plot.Y+1][plot.X], 0, 1, side, 3, region);
                        }
                        side++;
                    }
                }

                var perimeter = plots.SelectMany(p => p.PerimeterSides).Select(p => p).Where(p => p != -1).Distinct().Count();

                var area = plots.Count();

                price += perimeter*area;
            }

            return price;
        }

        // walk a perimeter "side" in one direction until it ends, marking those edges as part of the same side
        private void WalkSide(Plot p, int xDir, int yDir, int side, int perimiterSide, int region)
        {
            if (p.Region != region) return;
            if (!p.CountPerimeters[perimiterSide] || p.PerimeterSides[perimiterSide] != -1) return;

            p.PerimeterSides[perimiterSide] = side;

            if (xDir != 0 && p.X+xDir >= 0 && p.X+xDir < Map[0].Length)
                WalkSide(Map[p.Y][p.X+xDir], xDir, 0, side, perimiterSide, region);
            else if (yDir != 0 && p.Y+yDir >= 0 && p.Y+yDir < Map.Length)
                WalkSide(Map[p.Y+yDir][p.X], 0, yDir, side, perimiterSide, region);
        }
    }

    public class Plot
    {
        public char Type;
        public int? Region;
        public int Y;
        public int X;
        public bool[] CountPerimeters = new bool[4] { true, true, true, true };         // down, left, up, right
        public int[] PerimeterSides = new int[4] { -1, -1, -1, -1 };                    // for part 2
    }

    protected override long Part1()
    {
        var nextRegion = 1;
        for (int i = 0; i < Input.Map.Length; i++)
        {
            for (int j = 0; j < Input.Map[i].Length; j++)
            {
                // if this plot doesn't yet have a region yet, start it and flood fill.
                if (Input.Map[i][j].Region != null) continue;

                FloodFill(i, j, nextRegion++);
            }
        }

        return Input.Price();
    }

    protected override long Part2()
    {
        return Input.BulkPrice();
    }


    private void FloodFill(int i, int j, int region)
    {
        if (Input.Map[i][j].Region != null) return;

        Input.Map[i][j].Region = region;

        // check for connected Region in every direction to nullify perimiter
        if (i > 0 && Input.Map[i - 1][j].Region == region)
        {
            Input.Map[i][j].CountPerimeters[0] = false;
            Input.Map[i - 1][j].CountPerimeters[2] = false;
        }
        if (j > 0 && Input.Map[i][j - 1].Region == region)
        {
            Input.Map[i][j].CountPerimeters[3] = false;
            Input.Map[i][j - 1].CountPerimeters[1] = false;
        }
        if (i < Input.Map.Length - 1 && Input.Map[i + 1][j].Region == region)
        {
            Input.Map[i][j].CountPerimeters[2] = false;
            Input.Map[i + 1][j].CountPerimeters[0] = false;
        }
        if (j < Input.Map[i].Length - 1 && Input.Map[i][j + 1].Region == region)
        {
            Input.Map[i][j].CountPerimeters[1] = false;
            Input.Map[i][j + 1].CountPerimeters[3] = false;
        }

        if (i > 0 && Input.Map[i - 1][j].Type == Input.Map[i][j].Type)
        {
            FloodFill(i - 1, j, region);
        }
        if (j > 0 && Input.Map[i][j - 1].Type == Input.Map[i][j].Type)
        {
            FloodFill(i, j - 1, region);
        }
        if (i < Input.Map.Length - 1 && Input.Map[i + 1][j].Type == Input.Map[i][j].Type)
        {
            FloodFill(i + 1, j, region);
        }
        if (j < Input.Map[i].Length - 1 && Input.Map[i][j + 1].Type == Input.Map[i][j].Type)
        {
            FloodFill(i, j + 1, region);
        }
    }

    protected override Garden Parse(string input)
    {
        char[][] map = input.Split('\n').Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
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