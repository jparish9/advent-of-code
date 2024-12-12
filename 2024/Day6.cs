namespace AOC.AOC2024;

public class Day6 : Day<Day6.Map>
{
    protected override string? SampleRawInput { get => "....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#..."; }

    public class Map
    {
        public required char[][] Area;
        public int Height;
        public int Width;
        public required GuardState GuardState;
        public List<GuardState> GuardStateLog = new();

        public void Print()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Console.Write(Area[i][j]);
                }
                System.Console.WriteLine("Guard at " + GuardState.Position + " facing " + GuardState.Facing);
                Console.WriteLine();
            }
        }

        public MoveResult TryMove()
        {
            var (x, y) = GuardState.NewPosition();
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return MoveResult.OffMap;
            }

            if (Area[y][x] == '#')
            {
                GuardState = GuardState.TurnRight();
            }
            else
            {
                GuardState = GuardState.CompleteMove();
            }

            var result = GuardStateLog.Contains(GuardState) ? MoveResult.EnteredLoop : MoveResult.Success;
            GuardStateLog.Add(GuardState);
            return result;
        }
    }

    public class GuardState : IEquatable<GuardState>
    {
        public (int x, int y) Position;
        public (int x, int y) Facing;



        public (int x, int y) NewPosition() => (Position.x + Facing.x, Position.y + Facing.y);

        public GuardState TurnRight()
        {
            return new GuardState() {
                Position = Position,
                Facing = (-Facing.y, Facing.x)
            };
        }

        public GuardState CompleteMove()
        {
            return new GuardState() {
                Position = NewPosition(),
                Facing = Facing
            };
        }

        public bool Equals(GuardState? other)
        {
            if (other is null) return false;
            return Position == other.Position && Facing == other.Facing;
        }
    }

    public enum MoveResult { Success, OffMap, EnteredLoop };

    protected override Answer Part1()
    {
        while (Input.TryMove() != MoveResult.OffMap);
        return Input.GuardStateLog.Select(p => p.Position).Distinct().Count();
    }

    protected override Answer Part2()
    {
        // iterate over visited positions (MINUS THE STARTING ONE) to place an obstacle and see if we enter a loop.
        // my first attempt, putting an obstacle at each visited position and running the guard from its initial position, got the correct answer but took almost 4 minutes.
        // it also got slower as it progressed, because replacing later visited nodes with obstacles retraced all of the previous steps.

        // it occurred to me that we don't need to start at the beginning each time, since a loop doesn't necessarily mean a complete cycle.
        // at each point in the full visited path from part 1, try an obstacle right in front of the guard (if one isn't already there) and that becomes the start of a new path.
        // if that path loops, it is a loop in the full original path too.
        // this is under a minute.  there may be further improvements possible but this is definitely good enough.

        var origStateLog = Input.GuardStateLog.Select(p => new GuardState() { Position = p.Position, Facing = p.Facing }).ToList();      // make copy
        var loopObstacles = 0;

        foreach (var state in origStateLog)
        {
            var (x, y) = state.NewPosition();
            if (x < 0 || x >= Input.Width || y < 0 || y >= Input.Height) continue;
            if (Input.Area[y][x] == '#' || Input.Area[y][x] == 'C') continue;

            // try from this guard state with new obstacle at x,y (right in front of the guard)
            Input.GuardState = state;
            Input.GuardStateLog.Clear();
            Input.GuardStateLog.Add(state);
            Input.Area[y][x] = '#'; 

            var thisMapMoves = 0;
            while (true)
            {
                var result = Input.TryMove();
                if (result == MoveResult.OffMap) break;
                if (result == MoveResult.EnteredLoop) {
                    loopObstacles++;
                    break;
                }
                thisMapMoves++;
            }
            Input.Area[y][x] = 'C';     // mark checked!  don't double-count!  if we visit a place from two directions, placing an obstacle there (only) the second time is impossible.
        }

        return loopObstacles;
    }

    protected override Map Parse(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var area = new char[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            area[i] = lines[i].ToCharArray();
        }

        var guard = area.SelectMany(x => x).Select((x, i) => (x, i)).First(x => x.x == '^').i;      // find ^ in the input as a single array

        var initialGuardState = new GuardState() {
            Position = (guard % lines[0].Length, guard / lines[0].Length),
            Facing = (0, -1)            // initally facing up in both sample and input
        };

        return new Map() {
            Area = area,
            Height = lines.Length,
            Width = lines[0].Length,
            GuardState = initialGuardState,
            GuardStateLog = new () { initialGuardState }
        };
    }
}