namespace AOC.AOC2024;

public class Day15 : Day<Day15.Map>
{
    // large test from problem
    protected override string? SampleRawInput { get => "##########\n#..O..O.O#\n#......O.#\n#.OO..O.O#\n#..O@..O.#\n#O#..O...#\n#O..O..O.#\n#.OO.O.OO#\n#....O...#\n##########\n\n<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^\nvvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v\n><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<\n<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^\n^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><\n^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^\n>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^\n<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>\n^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>\nv^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^"; }
    // my test case that was failing!  the bottom of the diamond was getting moved twice (not checking for id uniqueness)
    //protected override string? SampleRawInput { get => "#######\n#.....#\n#..O..#\n#.OO..#\n#.O.O.#\n#@OO..#\n#..O..#\n#.....#\n#.....#\n#######\n\n>><^^>^^>>v"; }

    private static readonly Dictionary<char, (int dx, int dy)> DirectionMap = new()
    {
        { '^', (0, -1) },
        { 'v', (0, 1) },
        { '<', (-1, 0) },
        { '>', (1, 0) }
    };

    public class Obstacle
    {
        public enum Type { Box, Wall };

        public int Id;
        public Type ObstacleType;
        public int X;               // left side if ExtraX > 0
        public int Y;
        public int ExtraX;          // 0 if this takes up one coordinate, 1 if it takes up (X,Y) and (X+1,Y)

        // get a moved copy for testing collisions
        public Obstacle MoveCopy(int dx, int dy)
        {
            return new Obstacle() { ObstacleType = ObstacleType, Id = Id, X = X + dx, Y = Y + dy, ExtraX = ExtraX };
        }
    }

    public class Map
    {
        public required char[][] OrigWarehouse;

        public int Width;
        public int Height;

        public (int X, int Y) RobotPosition;

        public required List<(int x, int y)> Directions;

        public List<Obstacle> Obstacles = [];

        public int Step = 0;

        private readonly List<(Obstacle box, int dx, int dy)> BoxMoves = [];             // keep track of moves so we can commit them separately

        public List<Obstacle> Intersect(Obstacle o)
        {
            return Obstacles.Where(obs => obs.Id != o.Id && obs.X <= o.X + o.ExtraX && obs.X + obs.ExtraX >= o.X && obs.Y <= o.Y && obs.Y >= o.Y).ToList();
        }

        public List<Obstacle> Intersect(int x, int y, int ex)
        {
            return Obstacles.Where(obs => obs.X <= x + ex && obs.X + obs.ExtraX >= x && obs.Y <= y && obs.Y >= y).ToList();
        }

        public void PrintMap()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (RobotPosition.X == x && RobotPosition.Y == y)
                    {
                        System.Console.Write("@");
                        continue;
                    }

                    var obs = Obstacles.FirstOrDefault(o => o.X == x && o.Y == y);
                    if (obs == null)
                    {
                        System.Console.Write(".");
                    }
                    else
                    {
                        if (obs.ExtraX > 0)
                        {
                            System.Console.Write(obs.ObstacleType == Obstacle.Type.Box ? "[]" : "##");
                            x++;
                        }
                        else
                        {
                            System.Console.Write(obs.ObstacleType == Obstacle.Type.Box ? "O" : "#");
                        }
                    }
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("Robot at " + RobotPosition);
        }

        public void Reset(bool doubleWide)
        {
            Obstacles.Clear();
            Width = OrigWarehouse[0].Length * (doubleWide ? 2 : 1);
            Height = OrigWarehouse.Length;
            Step = 0;

            var idCt = 0;
            for (var y=0; y<OrigWarehouse.Length; y++)
            {
                for (var x=0; x<OrigWarehouse[0].Length; x++)
                {
                    if (OrigWarehouse[y][x] == 'O')
                    {
                        Obstacles.Add(new Obstacle() { ObstacleType = Obstacle.Type.Box, Id = idCt++, X = x * (doubleWide ? 2 : 1), Y = y, ExtraX = doubleWide ? 1 : 0 });
                    }
                    else if (OrigWarehouse[y][x] == '#')
                    {
                        Obstacles.Add(new Obstacle() { ObstacleType = Obstacle.Type.Wall, Id = idCt++, X = x * (doubleWide ? 2 : 1), Y = y, ExtraX = doubleWide ? 1 : 0 });
                    }
                }
            }

            var i = OrigWarehouse.SelectMany(p => p).Select((c, i) => (c, i)).First(p => p.c == '@');
            RobotPosition = (i.i % OrigWarehouse[0].Length * (doubleWide ? 2 : 1), i.i / OrigWarehouse[0].Length);
        }

        public void Move()
        {
            Directions.ForEach(d => MoveStep());
        }

        public void MoveStep()
        {
            var (dx, dy) = Directions[Step++];
            var (nx, ny) = (RobotPosition.X + dx, RobotPosition.Y + dy);
            var intersect = Intersect(nx, ny, 0);

            // open space, commit move
            if (intersect.Count == 0)
            {
                RobotPosition = (nx, ny);
                return;
            }

            // ran into wall(s)
            if (intersect.Any(p => p.ObstacleType == Obstacle.Type.Wall)) return;

            // ran into box(es), recursively try to move them, only commit moves if the whole chain would be successful
            if (TryMoveBoxes(intersect, dx, dy))
            {
                CommitMoves();
                RobotPosition = (nx, ny);
            }

            BoxMoves.Clear();
        }

        // returns true if the given list of boxes can all be (recursively) moved in the given direction
        private bool TryMoveBoxes(List<Obstacle> obstacles, int dx, int dy)
        {
            if (obstacles.Count == 0) return true;
            foreach (var obstacle in obstacles)
            {
                var intersect = Intersect(obstacle.MoveCopy(dx, dy));
                if (intersect.Any(p => p.ObstacleType == Obstacle.Type.Wall)) return false;

                if (!TryMoveBoxes(intersect, dx, dy)) return false;

                if (!BoxMoves.Any(p => p.box.Id == obstacle.Id))           // uniqueness check!  two boxes could both be moving this one
                    BoxMoves.Add((obstacle, dx, dy));
            }
            return true;
        }

        public long GPS()
        {
            var gps = 0L;
            foreach (var box in Obstacles.Where(o => o.ObstacleType == Obstacle.Type.Box))
            {
                gps += 100*box.Y + box.X;
            }
            return gps;
        }

        private void CommitMoves()
        {
            foreach (var (box, dx, dy) in BoxMoves)
            {
                box.X += dx;
                box.Y += dy;
            }
            BoxMoves.Clear();
        }
    }

    protected override Answer Part1()
    {
        Input.Reset(false);
        Input.Move();
        return Input.GPS();
    }

    protected override Answer Part2()
    {
        // part 2 necessitated basically a complete rewrite, going from a simple char grid to an obstacle list with intersect checking.
        // I had several unsuccessful attempts here, but ultimately a test case (not in the sample input) was failing,
        // where if two boxes were pushing the same box, that box would get moved twice.

        Input.Reset(true);
        Input.Move();
        return Input.GPS();
    }

    protected override Map Parse(string input)
    {
        var parts = input.Split("\n\n");
        var warehouseRows = parts[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var warehouse = new char[warehouseRows.Length][];
        for (int y = 0; y < warehouseRows.Length; y++)
        {
            warehouse[y] = warehouseRows[y].ToCharArray();
        }

        var map = new Map
        {
            OrigWarehouse = warehouse,
            Directions = parts[1].Replace("\n","").Select(c => DirectionMap[c]).ToList()
        };
        return map;
    }
}
