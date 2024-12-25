namespace AOC.AOC2023;

public class Day14 : Day<Day14.RockField>
{
    protected override string? SampleRawInput { get => "O....#....\nO.OO#....#\n.....##...\nOO.#O....O\n.O.....O#.\nO.#..O.#.#\n..O..#O..O\n.......O..\n#....###..\n#OO..#...."; }

    public class RockField
    {
        public required char[][] Grid { get; set; }

        public RockField Copy()
        {
            return new RockField() { Grid = Grid.Select(p => p.ToArray()).ToArray() };
        }

        public void Tilt(int xDir, int yDir)
        {
            if (yDir != 0)
            {
                // slide all O in given yDir until they hit the edge of the grid or are blocked by # or another O
                for (var x = 0; x < Grid[0].Length; x++)
                {
                    for (var y = yDir == -1 ? 1 : Grid.Length-1; (yDir == -1 && y < Grid.Length) || (yDir == 1 && y >= 0); y-=yDir)
                    {
                        if (Grid[y][x] == 'O')
                        {
                            // roll through all empty space
                            var moveToY = y;
                            while (moveToY+yDir >= 0 && moveToY+yDir < Grid.Length && Grid[moveToY+yDir][x] == '.')
                            {
                                moveToY += yDir;
                            }

                            if (moveToY == y) continue;

                            Grid[moveToY][x] = 'O';
                            Grid[y][x] = '.';
                        }
                    }
                }
            }
            else
            {
                // slide all O in given xDir until they hit the edge of the grid or are blocked by # or another O
                for (var y = 0; y < Grid.Length; y++)
                {
                    for (var x = xDir == -1 ? 1 : Grid[y].Length-1; (xDir == -1 && x < Grid[y].Length) || (xDir == 1 && x >= 0); x-=xDir)
                    {
                        if (Grid[y][x] == 'O')
                        {
                            // roll through all empty space
                            var moveToX = x;
                            while (moveToX+xDir >= 0 && moveToX+xDir < Grid[y].Length && Grid[y][moveToX+xDir] == '.')
                            {
                                moveToX += xDir;
                            }

                            if (moveToX == x) continue;

                            Grid[y][moveToX] = 'O';
                            Grid[y][x] = '.';
                        }
                    }
                }
            }
        }

        public int CalculateLoad()
        {
            var load = 0;

            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] == 'O')
                    {
                        load += Grid.Length-y;
                    }
                }
            }

            return load;
        }
    }

    protected override Answer Part1()
    {
        var copy = Input.Copy();
        copy.Tilt(0, -1);

        return copy.CalculateLoad();
    }

    protected override Answer Part2()
    {
        var copy = Input.Copy();

        // obviously we can't brute-force one BILLION spin cycles.
        // with some testing, it looks like the load eventually stabilizes and then just cycles through N values.
        // for the sample input, this N is 7.  for the larger input, we need to determine it.

        var loads = new List<int>();
        var cycleLengths = new List<int>();
        var foundLoad = -1;

        for (var i=1; i<10000; i++)
        {
            // spin cycle!
            copy.Tilt(0, -1);
            copy.Tilt(-1, 0);
            copy.Tilt(0, 1);
            copy.Tilt(1, 0);

            // check if we've seen this load before
            var load = copy.CalculateLoad();

            if (loads.Contains(load))
            {
                // we've seen this load before, so we may have hit the cycle.
                // store possible cycle length
                cycleLengths.Add(loads.Count - loads.LastIndexOf(load));
                loads.Add(load);

                // check for a complete cycle of the maximum [possible] cycle length we've seen so far, as there may of course be duplicate values
                var checkCycle = !cycleLengths.Any() ? 1 : cycleLengths.Max();
                if (checkCycle != 1)
                {
                    var cycle = true;
                    for (var j=1; j<=checkCycle; j++)
                    {
                        if (loads[^j] != loads[loads.Count-checkCycle-j])
                        {
                            cycle = false;
                            break;
                        }
                    }

                    if (cycle)
                    {
                        // we've found the cycle length!
                        // now we can calculate the load after the billionth spin cycle

                        // as an example, we have a cycle length of 7 starting at iteration 16.  (our array length is only 16.)
                        // what load between 10 and 16 (our found cycle) corresponds to after spin-cycle iteration 50?
                        // 16, 23, 30, 37, 44.  need +6 from 44 which is =16, but first subtract 7.  want load 16-7+6 = 15.  6 is not 50 % 7, but (50-16) % 7.
                        // more generally, for current load i, cycle length c, and a target spin cycle, we want load i + ((target - i) % c).
                        // to get that inside our array and known cycle, we then subtract the cycle length.
                        // finally, we subtract 1 because our array is 0-based.

                        foundLoad = loads[i + ((1000000000-i) % checkCycle) - checkCycle - 1];
                        break;
                    }
                }
            }
            else
            {
                loads.Add(load);
            }
        }

        if (foundLoad == -1)
            Console.WriteLine("Did not find a cycle after 10000 spin cycles!");
        return foundLoad;
    }

    protected override RockField Parse(RawInput input)
    {
        var lines = input.Lines().ToList();
        var grid = new char[lines.Count][];

        for (int i = 0; i < lines.Count; i++)
        {
            grid[i] = lines[i].ToCharArray();
        }

        return new RockField() { Grid = grid };
    }
}