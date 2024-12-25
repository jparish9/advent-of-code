namespace AOC.AOC2022;

public class Day14 : Day<Day14.Cave>
{
    protected override string? SampleRawInput { get => "498,4 -> 498,6 -> 496,6\n503,4 -> 502,4 -> 502,9 -> 494,9"; }

    public class Cave
    {
        public required char[][] Grid;

        public (int X, int Y) SandStart;

        public void Reset()
        {
            for (var y = 0; y < Grid.Length; y++)
            {
                for (var x = 0; x < Grid[y].Length; x++)
                {
                    if (Grid[y][x] == 'O')
                        Grid[y][x] = '.';
                }
            }
        }

        public (int X, int Y) Settle()
        {
            var pos = SandStart;
            while (true)
            {
                if (pos.Y == Grid.Length - 1) break;      // into the void

                if (Grid[pos.Y + 1][pos.X] == '.')
                {
                    pos = (pos.X, pos.Y + 1);
                }
                else if (Grid[pos.Y + 1][pos.X - 1] == '.')
                {
                    pos = (pos.X - 1, pos.Y + 1);
                }
                else if (Grid[pos.Y + 1][pos.X + 1] == '.')
                {
                    pos = (pos.X + 1, pos.Y + 1);
                }
                else break;
            }

            if (pos.Y < Grid.Length - 1)
                Grid[pos.Y][pos.X] = 'O';

            return pos;
        }
    }

    protected override Answer Part1()
    {
        var ct = 0;
        while (true)
        {
            if (Input.Settle().Y == Input.Grid.Length - 1) break;          // into the void
            ct++;
        }

        return ct;
    }

    protected override Answer Part2()
    {
        Input.Reset();
        
        // add a floor
        for (var x=0; x<Input.Grid[0].Length; x++)
        {
            Input.Grid[^1][x] = '#';
        }

        var ct = 0;
        while (true)
        {
            var pos = Input.Settle();

            ct++;
            if (pos == Input.SandStart) break;                  // no more grains possible
        }

        return ct;
    }

    protected override Cave Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();
        var allSegments = new List<(int x1, int y1, int x2, int y2)>();

        foreach (var line in lines)
        {
            var points = line.Split(" -> ");
            for (var i=0; i<points.Length-1; i++)
            {
                var p1 = points[i].Split(",");
                var p2 = points[i+1].Split(",");
                allSegments.Add((int.Parse(p1[0]), int.Parse(p1[1]), int.Parse(p2[0]), int.Parse(p2[1])));
            }
        }

        var maxX = allSegments.Max(p => Math.Max(p.x1, p.x2)) * 2;          // for floor in part 2
        var maxY = allSegments.Max(p => Math.Max(p.y1, p.y2)) + 2;          // check for falling indefinitely

        var grid = new char[maxY + 1][];
        for (var y = 0; y <= maxY; y++)
        {
            grid[y] = new char[maxX + 1];
            for (var x = 0; x <= maxX; x++)
            {
                grid[y][x] = '.';
            }
        }

        foreach (var (x1, y1, x2, y2) in allSegments)
        {
            if (x1 == x2)
            {
                for (var y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                {
                    grid[y][x1] = '#';
                }
            }
            else
            {
                for (var x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                {
                    grid[y1][x] = '#';
                }
            }
        }   

        return new Cave() { Grid = grid, SandStart = (500, 0) };
    }
}