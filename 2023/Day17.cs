using AOC.Utils;

namespace AOC.AOC2023;

public class Day17 : Day<Day17.Map>
{
    protected override string? SampleRawInput { get => "2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533"; }

    //protected override string? SampleRawInput { get => "111111111111\n999999999991\n999999999991\n999999999991\n999999999991"; }

    public class Map
    {
        public required int[][] Grid { get; set; }
    }

    protected override Answer Part1()
    {
        return AStarGridSearch.Search(
            () => (0, 0),
            () => new List<(int, int)> {(Input.Grid[0].Length-1, Input.Grid.Length-1)},
            (state) => true,
            (x, y) => Input.Grid[y][x],
            (currentNode) => GetNeighbors(currentNode, 0, 3)
        ).Last().G;
    }

    protected override Answer Part2()
    {
        return AStarGridSearch.Search(
            () => (0, 0),
            () => new List<(int, int)> {(Input.Grid[0].Length-1, Input.Grid.Length-1)},
            (state) => state % 11 >= 4,
            (x, y) => Input.Grid[y][x],
            (currentNode) => GetNeighbors(currentNode, 4, 10)
        ).Last().G;
    }

    private List<AStarGridSearch.Node> GetNeighbors(AStarGridSearch.Node currentNode, int minDirSteps, int maxDirSteps)
    {
        var adjacentNodes = new List<AStarGridSearch.Node>();
        var x = currentNode.Position.X;
        var y = currentNode.Position.Y;

        var divisor = maxDirSteps+1;

        // find neighbors
        // for part 1, we can go at most 3 steps in any direction, then we have to turn.
        // for part 2, we have to go at least 4 steps in one direction, but no more than 10 (then we must turn).
        // conveniently, node state helps a lot here.
        int? onlyDirAllowed = currentNode.Position == (0,0) || currentNode.State % divisor >= minDirSteps ? null : currentNode.State / divisor;

        var dirs = new List<(int X, int Y)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };
        for (var dirIndex=0; dirIndex<4; dirIndex++)
        {
            if (onlyDirAllowed.HasValue && dirIndex != onlyDirAllowed) continue;
            var (X, Y) = dirs[dirIndex];

            var newX = x + X;
            var newY = y + Y;

            if (newX < 0 || newY < 0 || newY >= Input.Grid.Length || newX >= Input.Grid[0].Length) continue;           // out of bounds
            if (newY == 0  && newX == 0) continue;            // don't revisit start
            if (newX == currentNode.Parent?.Position.X && newY == currentNode.Parent?.Position.Y) continue;    // don't turn around

            // check max straight-line constraint
            var stepsInThisDir = currentNode.State / divisor == dirIndex ? currentNode.State % divisor : 0;
            if (stepsInThisDir >= maxDirSteps) continue;

            // allowable neighbor
            var newNode = new AStarGridSearch.Node() { Position = (newX, newY), Parent = currentNode, State = divisor*dirIndex+stepsInThisDir+1};
            adjacentNodes.Add(newNode);
        }

        return adjacentNodes;
    }

    protected override Map Parse(string input)
    {
        var grid = input.Split('\n').Select(p => p.Trim()).Where(p => p != "").Select(p => p.Select(q => int.Parse(q.ToString())).ToArray()).ToArray();
        return new Map() { Grid = grid };
    }
}