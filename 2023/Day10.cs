namespace AOC.AOC2023;

public class Day10 : Day<char[][]>
{
    protected override string? SampleRawInput { get => "7-F7-\nLFJ|7\nSJLL7\n|F--J\nLJ.LJ"; }
    protected override string? SampleRawInputPart2 { get => "FF7FSF7F7F7F7F7F---7\nL|LJ||||||||||||F--J\nFL-7LJLJ||||||LJL-77\nF--JF--7||LJLJ7F7FJ-\nL---JF-JLJ.||-FJLJJ7\n|F|F-JF---7F7-L7L|7|\n|FFJF7L7F-JF7|JL---7\n7-L-JL7||F7|L7F-7F7|\nL.L7LFJ|||||FJL7||LJ\nL7JLJL-JLJLJL--JLJ.L"; }

    //private readonly Dictionary<int, HashSet<int>> _pathMap = new();     // need to look up if a coordinate is on the path often, so store it for efficient lookup
    private readonly List<Tuple<int, int>> _orderedPath = new();        // need path order for part 2

    protected override long Part1()
    {
        DeterminePath();
        return _orderedPath.Count / 2;
    }

    protected override long Part2()
    {
        DeterminePath();

        // well, after trying to get count crossings without success, I found out about the shoelace formula and learned something new today!
        // https://en.wikipedia.org/wiki/Shoelace_formula
        // a formula using only matrix determinants of ordered vertices to calculate the area of a polygon.
        // then apply Pick's theorem to get the number of interior points, since all of the vertices are lattice points.

        // another option is to expand the grid so that there are no "squeezed" pipes and use a flood fill algorithm to count the area, but this is so succinct!

        var detSum = 0;
        for (var i=0; i<_orderedPath.Count; i++)
        {
            detSum += _orderedPath[i].Item1 * _orderedPath[(i+1)%_orderedPath.Count].Item2 - _orderedPath[i].Item2 * _orderedPath[(i+1)%_orderedPath.Count].Item1;
        }

        detSum = Math.Abs(detSum);      // will be negative if the path is clockwise, positive if counter-clockwise

        var area = detSum/2;

        // using Pick's theorem, area = i + b/2 - 1 where b is the number of boundary points, i is the number of interior points (which we want)
        // i = area - b/2 + 1
        return area - _orderedPath.Count/2 + 1;
    }

    private void DeterminePath()
    {
        //_pathMap.Clear();
        _orderedPath.Clear();

        var x=0;
        var y=0;

        // find the starting point
        for (; y<Input.Length; y++)
        {
            for (x=0; x<Input[y].Length; x++)
            {
                if (Input[y][x] == 'S')
                {
                    break;
                }
            }
            if (x < Input[y].Length)
            {
                break;
            }
        }

        var xStart = x;
        var yStart = y;

        //_pathMap.Add(y, new HashSet<int>() {x});
        _orderedPath.Add(new Tuple<int, int>(y, x));

        var xDir = 0;
        var yDir = 0;

        // determine a valid initial direction
        if (x+1 < Input[y].Length && new char[]{'-', 'J', '7'}.Contains(Input[y][x+1]))
            xDir = 1;
        else if (x-1 >= 0 && new char[]{'-', 'L', 'F'}.Contains(Input[y][x-1]))
            xDir = -1;
        else if (y+1 < Input.Length && new char[]{'|', 'L', 'J'}.Contains(Input[y+1][x]))
            yDir = 1;
        else if (y-1 >= 0 && new char[]{'|', '7', 'F'}.Contains(Input[y-1][x]))
            yDir = -1;

        x += xDir;
        y += yDir;

        while (x != xStart || y != yStart)
        {
            //if (!_pathMap.ContainsKey(y)) _pathMap.Add(y, new HashSet<int>() {x});
            //else _pathMap[y].Add(x);
            _orderedPath.Add(new Tuple<int, int>(y, x));

            char next = Input[y][x];
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
    }

    protected override char[][] Parse(string input)
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

        return map;
    }
}