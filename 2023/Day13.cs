namespace AOC.AOC2023;

public class Day13 : Day<List<Day13.Grid>>
{
    protected override string? SampleRawInput { get => "#.##..##.\n..#.##.#.\n##......#\n##......#\n..#.##.#.\n..##..##.\n#.#.##.#.\n\n#...##..#\n#....#..#\n..##..###\n#####.##.\n#####.##.\n..##..###\n#....#..#"; }

    public class Grid
    {
        public required char[][] Map { get; set; }

        public int GetReflection(int ignoreReflection = -1)
        {
            var total = 0;
            var isValid = true;

            // test vertical lines
            for (var x=1; x<=Map[0].Length-1; x++)
            {
                isValid = true;
                for (var y=0; y<Map.Length; y++)
                {
                    for (var mirrorTest=0; x+mirrorTest<Map[0].Length && x-mirrorTest-1 >= 0; mirrorTest++)
                    {
                        if (Map[y][x+mirrorTest] != Map[y][x-mirrorTest-1])
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid) break;
                }

                if (isValid)
                {
                    if (x == ignoreReflection) { isValid = false; continue; }

                    total += x;
                    break;
                }
            }

            if (isValid) return total;

            // test horizontal lines
            for (var y=1; y<=Map.Length-1; y++)
            {
                isValid = true;
                for (var x=0; x<Map[0].Length; x++)
                {
                    for (var mirrorTest=0; y+mirrorTest<Map.Length && y-mirrorTest-1 >= 0; mirrorTest++)
                    {
                        if (Map[y+mirrorTest][x] != Map[y-mirrorTest-1][x])
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid) break;
                }

                if (isValid)
                {
                    if (100*y == ignoreReflection) { continue; }

                    total += 100*y;
                    break;
                }
            }

            /*if (!isValid)
            {
                Console.WriteLine("No line found!");
                Console.WriteLine(string.Join('\n', Map.Select(p => string.Join("", p))));
            }*/

            return total;
        }
    }

    protected override Answer Part1()
    {
        return Input.Sum(p => p.GetReflection());
    }

    protected override Answer Part2()
    {
        var total = 0;

        foreach (var grid in Input)
        {
            var currentReflection = grid.GetReflection();

            var foundDifferent = false;

            // "smudge" grid-points until we find a different (valid) reflection
            for (var y=0; y<grid.Map.Length; y++)
            {
                for (var x=0; x<grid.Map[0].Length; x++)
                {
                    var original = grid.Map[y][x];
                    grid.Map[y][x] = original == '.' ? '#' : '.';

                    var newReflection = grid.GetReflection(currentReflection);
                    if (newReflection != 0)
                    {
                        total += newReflection;
                        foundDifferent = true;
                        break;
                    }

                    grid.Map[y][x] = original;
                }
                if (foundDifferent) break;
            }

            /*if (!foundDifferent)
            {
                System.Console.WriteLine("No different reflection found!");
                System.Console.WriteLine(string.Join('\n', grid.Map.Select(p => string.Join("", p))));
            }*/
        }

        return total;
    }

    protected override List<Grid> Parse(RawInput input)
    {
        var result = new List<Grid>();
        var grids = input.LineGroups();

        foreach (var grid in grids)
        {
            var map = new char[grid.Count][];

            for (var i=0; i<grid.Count; i++)
            {
                map[i] = grid[i].ToCharArray();
            }

            result.Add(new Grid { Map = map });
        }

        return result;

    }
}