using AOC.Utils;

namespace AOC.AOC2023;

public class Day10 : Day<Day10.PipeMaze>
{
    protected override string? SampleRawInput { get => "7-F7-\nLFJ|7\nSJLL7\n|F--J\nLJ.LJ"; }
    protected override string? SampleRawInputPart2 { get => "FF7FSF7F7F7F7F7F---7\nL|LJ||||||||||||F--J\nFL-7LJLJ||||||LJL-77\nF--JF--7||LJLJ7F7FJ-\nL---JF-JLJ.||-FJLJJ7\n|F|F-JF---7F7-L7L|7|\n|FFJF7L7F-JF7|JL---7\n7-L-JL7||F7|L7F-7F7|\nL.L7LFJ|||||FJL7||LJ\nL7JLJL-JLJLJL--JLJ.L"; }

    public class PipeMaze
    {
        public required char[][] Map { get; set; }

        public List<(long, long)> DeterminePath()
        {
            var orderedPath = new List<(long, long)>();

            var x=0;
            var y=0;

            // find the starting point
            for (; y<Map.Length; y++)
            {
                for (x=0; x<Map[y].Length; x++)
                {
                    if (Map[y][x] == 'S')
                    {
                        break;
                    }
                }
                if (x < Map[y].Length)
                {
                    break;
                }
            }

            var xStart = x;
            var yStart = y;

            orderedPath.Add((y, x));

            var xDir = 0;
            var yDir = 0;

            // determine a valid initial direction
            if (x+1 < Map[y].Length && new char[]{'-', 'J', '7'}.Contains(Map[y][x+1]))
                xDir = 1;
            else if (x-1 >= 0 && new char[]{'-', 'L', 'F'}.Contains(Map[y][x-1]))
                xDir = -1;
            else if (y+1 < Map.Length && new char[]{'|', 'L', 'J'}.Contains(Map[y+1][x]))
                yDir = 1;
            else if (y-1 >= 0 && new char[]{'|', '7', 'F'}.Contains(Map[y-1][x]))
                yDir = -1;

            x += xDir;
            y += yDir;

            while (x != xStart || y != yStart)
            {
                //if (!_pathMap.ContainsKey(y)) _pathMap.Add(y, new HashSet<int>() {x});
                //else _pathMap[y].Add(x);
                orderedPath.Add((y, x));

                char next = Map[y][x];
                //System.Console.WriteLine("at " + x + "," + y + ", found " + next);

                if (next == '|' || next == '-')
                {
                    // continue in same direction
                }
                else if (next == 'L')
                {
                    // L connects north and east, so bend in whatever direction we are going
                    if (xDir == 0)
                    {
                        xDir = yDir;
                        yDir = 0;
                    }
                    else
                    {
                        yDir = xDir;
                        xDir = 0;
                    }
                }
                else if (next == 'J')
                {
                    // J connects north and west, so bend in whatever direction we are going
                    if (xDir == 0)
                    {
                        xDir = -yDir;
                        yDir = 0;
                    }
                    else
                    {
                        yDir = -xDir;
                        xDir = 0;
                    }
                }
                else if (next == '7')
                {
                    // 7 connects south and west, so bend in whatever direction we are going
                    if (xDir == 0)
                    {
                        xDir = yDir;
                        yDir = 0;
                    }
                    else
                    {
                        yDir = xDir;
                        xDir = 0;
                    }
                }
                else if (next == 'F')
                {
                    // F connects south and east, so bend in whatever direction we are going
                    if (xDir == 0)
                    {
                        xDir = -yDir;
                        yDir = 0;
                    }
                    else
                    {
                        yDir = -xDir;
                        xDir = 0;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: ran off the map at " + x + "," + y + ", found " + next);
                }

                x += xDir;
                y += yDir;
            }

            return orderedPath;
        }
    }
    protected override long Part1()
    {
        return Input.DeterminePath().Count / 2;
    }

    protected override long Part2()
    {
        // well, after trying to get count crossings without success, I found out about the shoelace formula and learned something new today!
        // https://en.wikipedia.org/wiki/Shoelace_formula
        // a formula using only matrix determinants of ordered vertices to calculate the area of a polygon.
        // then apply Pick's theorem to get the number of interior points, since all of the vertices are lattice points.
        // another option is to expand the grid so that there are no "squeezed" pipes and use a flood fill algorithm to count the area, but this is so succinct!

        return Polygon.GridLatticePoints(Input.DeterminePath(), true);
    }

    protected override PipeMaze Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();
        char[][] map = new char[lines.Length][];
        for(var i=0; i<lines.Length; i++)
        {
            map[i] = new char[lines[i].Length];
            for (var j=0; j<lines[i].Length; j++)
            {
                map[i][j] = lines[i][j];
            }
        }

        return new PipeMaze() { Map = map };
    }
}