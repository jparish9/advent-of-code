using AOC.Utils;

namespace AOC.AOC2022;

public class Day12 : Day<Day12.Map>
{
    protected override string? SampleRawInput { get => "Sabqponm\nabcryxxl\naccszExk\nacctuvwj\nabdefghi"; }

    public class Map
    {
        public required char[][] Grid { get; set; }

        // start cache; GetNeighbors uses it more than once and is called many times.
        private (int, int)? _start;

        // "Start" is E because we are searching backwards
        public (int X, int Y) Start()
        {
            if (_start != null) return _start.Value;

            var found = Find('E', 1);
            if (!found.Any()) throw new Exception("No start found");

            _start = found.First();
            return _start.Value;
        }

        // find all possible ends
        public List<(int, int)> End(char endLevel)
        {
            return Find(endLevel);
        }

        private List<(int, int)> Find(char level, int? max = null)
        {
            var found = new List<(int, int)>();

            for (var y = 0; y < Grid.Length; y++)
            {
                for (var x = 0; x < Grid[0].Length; x++)
                {
                    if (Grid[y][x] == level)
                    {
                        found.Add((x, y));
                        if (max != null && found.Count >= max.Value) return found;
                    }
                }
            }

            return found;
        }
    }

    protected override Answer Part1()
    {
        return AStarGridSearch.Search(
            Input.Start,
            () => Input.End('S'),
            (state) => true,
            (current, dest) => 1,
            (currentNode) => GetNeighbors(currentNode, 'S')
        ).Last().G;
    }

    protected override Answer Part2()
    {
        return AStarGridSearch.Search(
            Input.Start,
            () => Input.End('a'),
            (state) => true,
            (current, dest) => 1,
            (currentNode) => GetNeighbors(currentNode, 'a')
        ).Last().G;
    }

    private List<AStarGridSearch.Node> GetNeighbors(AStarGridSearch.Node currentNode, char endLevel)
    {
        var adjacentNodes = new List<AStarGridSearch.Node>();
        var dirs = new [] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var x = currentNode.Position.X;
        var y = currentNode.Position.Y;

        foreach (var (X, Y) in dirs)
        {
            var newX = x + X;
            var newY = y + Y;

            if (newX < 0 || newY < 0 || newY >= Input.Grid.Length || newX >= Input.Grid[0].Length) continue;           // out of bounds
            if (newY == Input.Start().Y && newX == Input.Start().X) continue;                        // don't revisit start

            // if searching backwards, we can go down at most 1, but up any amount
            var currentLevel = Input.Grid[y][x] == 'E' ? 'z'+1 : Input.Grid[y][x];
            var neighborLevel = Input.Grid[newY][newX] == endLevel ? (endLevel == 'S' ? 'a'-1 : endLevel) : Input.Grid[newY][newX];
            
            if (neighborLevel < currentLevel-1) continue;

            // allowable neighbor
            var newNode = new AStarGridSearch.Node() { Position = (newX, newY) };
            adjacentNodes.Add(newNode);
        }

        return adjacentNodes;
    }

    protected override Map Parse(RawInput input)
    {
        var grid = input.Lines().Select(p => p.ToCharArray()).ToArray();
        return new Map() { Grid = grid };
    }
}