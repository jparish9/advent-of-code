namespace AOC.AOC2023;

public class Day16 : Day<Day16.Cave>
{
    protected override string? SampleRawInput { get => ".|...\\....\n|.-.\\.....\n.....|-...\n........|.\n..........\n.........\\\n..../.\\\\..\n.-.-/..|..\n.|....-|.\\\n..//.|...."; }

    public class Cave
    {
        public required char[][] Grid { get; set; }

        public required HashSet<(int X, int Y, int XDir, int YDir)> Energized { get; set; }

        public long NewBeam(int x, int y, int xDir, int yDir)
        {
            Energized.Clear();
            Beam(x, y, xDir, yDir);
            return Energized.Select(p => (p.X, p.Y)).Distinct().Count();
        }

        private void Beam(int x, int y, int xDir, int yDir)
        {
            if (Energized.Contains((x, y, xDir, yDir))) return;     // already been here

            // move and energize in the given direction as far as we can
            while (x >= 0 && y >= 0 && x < Grid[0].Length && y < Grid.Length)
            {
                Energized.Add((x, y, xDir, yDir));
                if (Grid[y][x] == '\\')
                {
                    // redirect down (0,1) to right (1,0), up (0,-1) to left (-1,0), right (1,0) to down (0,1), left (-1,0) to up (0,-1).  yDir=xDir, xDir=yDir.
                    (xDir, yDir) = (yDir, xDir);
                }
                else if (Grid[y][x] == '/')
                {
                    // redirect down (0,1) to left (-1,0), up (0,-1) to right (1,0), right (1,0) to up (0,-1), left (-1,0) to down (0,1).  yDir=-xDir, xDir=-yDir.
                    (xDir, yDir) = (-yDir, -xDir);
                }
                else if ((Grid[y][x] == '-' && xDir != 0)         // pass-through
                    || (Grid[y][x] == '|' && yDir != 0)
                    || (Grid[y][x] == '.')) {}
                else break;                                     // hit a splitter; handle below
                
                x += xDir;
                y += yDir;
            }

            if (x < 0 || y < 0 || y >= Grid.Length || x >= Grid[0].Length) return;      // ran off edge of grid

            // handle splitters
            if (Grid[y][x] == '-')            // && yDir != 0 (from above)
            {
                // split into left-right
                Beam(x-1, y, -1, 0);
                Beam(x+1, y, 1, 0);
            }
            else if (Grid[y][x] == '|')       // && xDir != 0 (from above)
            {
                // split into up-down
                Beam(x, y-1, 0, -1);
                Beam(x, y+1, 0, 1);
            }
        }
    }

    protected override Answer Part1()
    {
        // beam of light starts at 0,0 and travels right
        return Input.NewBeam(0, 0, 1, 0);
    }

    protected override Answer Part2()
    {
        var maxEnergized = 0L;

        for (var i=0; i<Input.Grid.Length; i++)
        {
            // beam right from left, left from right
            maxEnergized = Math.Max(maxEnergized, Input.NewBeam(0, i, 1, 0));
            maxEnergized = Math.Max(maxEnergized, Input.NewBeam(Input.Grid[0].Length-1, i, -1, 0));
        }

        for (var i=0; i<Input.Grid[0].Length; i++)
        {
            // beam down from top, up from bottom
            maxEnergized = Math.Max(maxEnergized, Input.NewBeam(i, 0, 0, 1));
            maxEnergized = Math.Max(maxEnergized, Input.NewBeam(i, Input.Grid.Length-1, 0, -1));
        }

        return maxEnergized;
    }

    protected override Cave Parse(RawInput input)
    {
        var grid = input.Lines().Select(p => p.ToCharArray()).ToArray();
        var energized = new bool[grid.Length][];
        for (var i = 0; i < grid.Length; i++)
        {
            energized[i] = new bool[grid[0].Length];
        }
        return new Cave() { Grid = grid, Energized = new HashSet<(int, int, int, int)>() };
    }
}