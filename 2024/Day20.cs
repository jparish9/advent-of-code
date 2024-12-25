namespace AOC.AOC2024;

public class Day20 : Day<Day20.Racetrack>
{
    protected override string? SampleRawInput { get => "###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############"; }

    public class Racetrack
    {
        public Racetrack(char[][] grid)
        {
            Grid = grid;

            // find start, "search" single path to end and record step count at each path step
            var start = (x: -1, y: -1);
            Steps = new int[Grid.Length][];
            for (var y=0; y<Grid.Length; y++)
            {
                Steps[y] = new int[Grid[y].Length];
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] == 'S') start = (x, y);
                    Steps[y][x] = -1;
                }
            }

            if (start == (-1, -1)) throw new Exception("Start not found");

            var pos = start;
            var steps = 0;
            Steps[pos.y][pos.x] = 0;
            while (Grid[pos.y][pos.x] != 'E')
            {
                foreach (var (X, Y) in GridCardinals)
                {
                    var newX = pos.x + X;
                    var newY = pos.y + Y;

                    if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;           // out of bounds
                    if (Grid[newY][newX] == '#') continue;                                                          // wall
                    if (Steps[newY][newX] != -1) continue;                                                          // already visited

                    // continue path
                    pos = (newX, newY);
                    Steps[pos.y][pos.x] = ++steps;
                    break;
                }
            }

            // find all cheats up to a maximum of 20 steps
            // a cheat is defined as a series of moves from a path point (including start) to another path point (including end) that is shorter than taking the full path between them.
            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] != '.' && Grid[y][x] != 'S') continue;            // on path (including start)

                    for (var newY=y-20; newY<=y+20; newY++)
                    {
                        for (var newX=x-20; newX<=x+20; newX++)
                        {
                            // end point checks
                            if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;           // out of bounds
                            if (Steps[newY][newX] == -1) continue;                                                          // not on path

                            var cheatDistance = Math.Abs(newX - x) + Math.Abs(newY - y);
                            if (cheatDistance > 20) continue;                                                               // manhattan distance > 20
                            var saved = Steps[newY][newX] - Steps[y][x] - cheatDistance;
                            if (saved <= 0) continue;                                                                       // not shorter than full path
                            
                            // valid cheat
                            Cheats.Add(new Cheat() { From = (x, y), To = (newX, newY), Steps = cheatDistance, Saved = saved });
                        }
                    }
                }
            }
        }

        private readonly char[][] Grid;
        private readonly int[][] Steps;           // -1 if not on path, otherwise number of steps taken at this point

        public List<Cheat> Cheats = [];
    }

    public class Cheat
    {
        public (int X, int Y) From;
        public (int X, int Y) To;
        public int Steps;                   // manhattan distance
        public int Saved;                   // steps saved over the full path between From and To
    }

    protected override Answer Part1()
    {
        return Input.Cheats.Count(p => p.Saved >= 100 && p.Steps == 2);
    }

    protected override Answer Part2()
    {
        return Input.Cheats.Count(p => p.Saved >= 100);
    }


    protected override Racetrack Parse(string input)
    {
        var grid = input.Split("\n").Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
        return new Racetrack(grid);
    }
}