using AOC.Utils;

namespace AOC.AOC2024;

public class Day18 : Day<Day18.MemorySpace>
{
    protected override string? SampleRawInput { get => "5,4\n4,2\n4,5\n3,0\n2,1\n6,3\n2,4\n1,5\n0,6\n3,3\n2,6\n5,1\n1,2\n5,5\n2,5\n6,5\n1,4\n0,4\n6,4\n1,1\n6,1\n1,0\n0,5\n1,6\n2,0"; }

    private static readonly (int X, int Y)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public class MemorySpace
    {
        public required List<(int X, int Y)> Corrupted;
        public int Width;
        public int Height;
        public int Take;                                      // how many corrupted bytes to use
        public List<AStarGridSearch.Node>? LastPath;          // save the path for part 2
    }

    protected override Answer Part1()
    {
        Input.LastPath = AStarGridSearch.Search(
            () => (0, 0),
            () => [(Input.Width-1, Input.Height-1)],
            (state) => true,
            (current, dest) => 1,
            (currentNode) => GetNeighbors(currentNode, Input.Take)
        );

        return Input.LastPath.Last().G;
    }

    protected override Answer Part2()
    {
        // we know the minimum to block the path is at least byte Take+1, and have saved the last path from using byte Take (part 1).
        // just iterate until we find it.  this takes ~5 minutes with no optimizations.
        // we can optimize by only re-searching the path when the new corrupted byte is on the last path.
        // for my input, this tries 1828 bytes but only has to re-search 36 of them, finishing in ~7 seconds.
        var take = Input.Take+1;
        while (true)
        {
            if (Input.LastPath!.Any(n => n.Position == Input.Corrupted[take-1]))
            {
                Input.LastPath = AStarGridSearch.Search(
                    () => (0, 0),
                    () => [(Input.Width-1, Input.Height-1)],
                    (state) => true,
                    (current, dest) => 1,
                    (currentNode) => GetNeighbors(currentNode, take));
                if (Input.LastPath.Count == 0) break;           // no path found
            }

            take++;
        }

        // return the byte that caused no path to be found.
        return Input.Corrupted[take-1].X + "," + Input.Corrupted[take-1].Y;
    }

    private List<AStarGridSearch.Node> GetNeighbors(AStarGridSearch.Node currentNode, int take)
    {
        var adjacentNodes = new List<AStarGridSearch.Node>();
        var x = currentNode.Position.X;
        var y = currentNode.Position.Y;

        foreach (var (X, Y) in Directions)
        {
            var newX = x + X;
            var newY = y + Y;

            if (newX < 0 || newX >= Input.Width || newY < 0 || newY >= Input.Height) continue;

            // check for corrupted
            if (Input.Corrupted.Take(take).Any(p => p == (newX, newY))) continue;

            var newNode = new AStarGridSearch.Node()
            {
                Position = (newX, newY)
            };

            adjacentNodes.Add(newNode);
        }

        return adjacentNodes;
    }

    protected override MemorySpace Parse(string input)
    {
        var corrupted = input.Split("\n").Where(p => p != "").Select(p => p.Split(",")).Select(p => (X: int.Parse(p[0]), Y: int.Parse(p[1]))).ToList();

        // 7x7 for sample (0-6), 71x71 for real input (0-70).
        var isSample = corrupted.Count < 30;
        return new MemorySpace()
        {
            Corrupted = corrupted,
            Width = isSample ? 7 : 71,
            Height = isSample ? 7 : 71,
            Take = isSample ? 12 : 1024
        };
    }
}