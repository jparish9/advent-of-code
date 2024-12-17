using AOC.Utils;

namespace AOC.AOC2024;

public class Day16 : Day<Day16.Map>
{
    // small example
    //protected override string? SampleRawInput { get => "###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############"; }
    // larger example
    protected override string? SampleRawInput { get => "#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################";}
    // this test case showed my bug in part 1; reaching a node from two different directions should not be considered the same node.
    //protected override string? SampleRawInput { get => "##########\n#.......E#\n#.##.#####\n#..#.....#\n##.#####.#\n#S.......#\n##########"; }
    //protected override string? SampleRawInput { get => "#######\n#S....#\n#.#.#.#\n#.....#\n#.#.#.#\n#.....#\n#....E#\n#######"; }

    private static readonly (int X, int Y)[] Directions = new[] { (0, -1), (1, 0), (0, 1), (-1, 0) };           // up, right, down, left

    public class Map
    {
        public Map(char[][] Grid)
        {
            this.Grid = Grid;

            // set start and end
            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] == 'S') Start = (x, y);
                    else if (Grid[y][x] == 'E') End = (x, y);
                }
            }


            // build edge map
            for (var y=0; y<Grid.Length; y++)
            {
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] == '#') continue;

                    for (var facingNdx=0; facingNdx<Directions.Length; facingNdx++)
                    {
                        AllNodes.Add(new Node() { X = x, Y = y, Facing = facingNdx });
                    }
                }
            }

            foreach (var node in AllNodes)
            {
                node.Edges.Add(new Edge() { ConnectedTo = AllNodes.First(p => p.X == node.X && p.Y == node.Y && p.Facing == (node.Facing + 1) % 4), Weight = 1000 });
                node.Edges.Add(new Edge() { ConnectedTo = AllNodes.First(p => p.X == node.X && p.Y == node.Y && p.Facing == (4 + (node.Facing - 1)) % 4), Weight = 1000 });

                // check one step in the current facing direction
                var (xDir, yDir) = Directions[node.Facing];
                var newX = node.X + xDir;
                var newY = node.Y + yDir;

                if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;
                if (newY == Start.Y && newX == Start.X) continue;                        // don't revisit start
                if (Grid[newY][newX] == '#') continue;           // wall

                node.Edges.Add(new Edge() { ConnectedTo = AllNodes.First(p => p.X == newX && p.Y == newY && p.Facing == node.Facing), Weight = 1 });
            }

            Reset();
        }

        private void Reset()
        {
            BestPathTiles = new();
        }

        public char[][] Grid;

        public Dictionary<(int X, int Y, int facing), List<(int X, int Y, int facing, int weight)>> Edges = new();

        public (int X, int Y) Start;
        public (int X, int Y) End;

        public long BestPath = long.MaxValue;

        public List<(int X, int Y)> BestPathTiles = new();

        public List<Node> AllNodes = new();
    }

    public class Node
    {
        public int X;
        public int Y;
        public int Facing;

        public List<Edge> Edges = new();
    }

    public class Edge
    {
        public required Node ConnectedTo;
        public int Weight;
    }

    protected override Answer Part1()
    {
        // djikstra's algorithm using Edges and starting at S (facing 1,0), ending at E (facing any)
        var start = Input.AllNodes.First(p => p.X == Input.Start.X && p.Y == Input.Start.Y && p.Facing == 1);

        var weights = Input.AllNodes.Select(p => (p, details: (Previous: (Node?)null, Weight: int.MaxValue))).ToDictionary(p => p.Item1, p => p.details);
        var queue = new PriorityQueue<Node, int>();

        weights[start] = (null, 0);

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.X == Input.End.X && current.Y == Input.End.Y)
            {
                return weights[current].Weight;
            }

            var currentWeight = weights[current].Weight;

            foreach (var edge in current.Edges)
            {
                //System.Console.WriteLine("Checking " + edge.ConnectedTo.X + "," + edge.ConnectedTo.Y + "," + edge.ConnectedTo.Facing);
                var weight = weights[edge.ConnectedTo].Weight;
                var newWeight = currentWeight + edge.Weight;

                if (newWeight < weight)
                {
                    //System.Console.WriteLine("Updating " + edge.ConnectedTo.X + "," + edge.ConnectedTo.Y + "," + edge.ConnectedTo.Facing + " to " + newWeight);
                    weights[edge.ConnectedTo] = (current, newWeight);
                    queue.Remove(edge.ConnectedTo, out _, out _);
                    queue.Enqueue(edge.ConnectedTo, newWeight);
                }
            }
        }

        System.Console.WriteLine("Didn't find a path!");
        return 0;

        // this A* search is much faster, but we need the cumulative weight of the path, not just the final node's weight.

        /*var path = AStarGridSearch.Search(
            () => Input.Start,
            () => new List<(int, int)> { Input.End },
            (state) => true,
            (current, movingTo) => {
                // should never be invoked if not one unit away (GetNeighbors).
                // we can use the facing direction (from the previous move) to determine the cost to move to the next node.
                var (facingX, facingY) = current.Parent == null ? (1, 0) : (current.Position.X - current.Parent.Position.X, current.Position.Y - current.Parent.Position.Y);
                if (current.Position.X + facingX == movingTo.Position.X && current.Position.Y + facingY == movingTo.Position.Y) return 1;
                else return 1001;    // requires changing direction first (+1000 cost)
            },
            GetNeighbors
        );

        Input.BestPath = path.Last().G;         // cache for part 2
        return Input.BestPath;*/
    }

    protected override Answer Part2()
    {
        // use a general DFS to find ALL paths that have the value of the best path.
        DFS2(new List<(int, int, int)>() { (Input.Start.X, Input.Start.Y, 1) }, 0, new HashSet<(int, int, int)>());
        return Input.BestPathTiles.Distinct().Count();
    }

    private void DFS2(List<(int X, int Y, int facing)> currentPath, int cumulativeCost, HashSet<(int X, int Y, int facing)> visited)
    {
        var last = currentPath[^1];
        //System.Console.WriteLine("Visiting " + last + " with cost " + cumulativeCost);
        if (last.X == Input.End.X && last.Y == Input.End.Y)
        {
            System.Console.WriteLine("Found a path to the end");
            if (cumulativeCost == Input.BestPath)
            {
                System.Console.WriteLine(" ... it's a best path");
                Input.BestPathTiles.AddRange(currentPath.Select(p => (p.X, p.Y)));
            }
            return;
        }

        visited.Add(last);

        foreach (var (X, Y, facing, weight) in Input.Edges[last])
        {
            var n = (X, Y, facing);
            if (!visited.Contains(n))
            {
                if (cumulativeCost + weight > Input.BestPath) continue;       // prune
                currentPath.Add(n);
                DFS2(currentPath, cumulativeCost + weight, visited);
                currentPath.Remove(n);
            }
        }

        visited.Remove(last);
    }

    private List<AStarGridSearch.Node> GetNeighbors(AStarGridSearch.Node currentNode)
    {
        var adjacentNodes = new List<AStarGridSearch.Node>();
        var dirs = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var x = currentNode.Position.X;
        var y = currentNode.Position.Y;

        foreach (var (X, Y) in dirs)
        {
            var newX = x + X;
            var newY = y + Y;

            if (newX < 0 || newY < 0 || newY >= Input.Grid.Length || newX >= Input.Grid[0].Length) continue;           // out of bounds
            if (newY == Input.Start.Y && newX == Input.Start.X) continue;                        // don't revisit start
            if (Input.Grid[newY][newX] == '#') continue;           // wall

            var dirReachedFrom = currentNode.Parent == null ? (1, 0) : (currentNode.Position.X - currentNode.Parent.Position.X, currentNode.Position.Y - currentNode.Parent.Position.Y);
            var dirIndex = dirs.IndexOf(dirReachedFrom);

            // allowable neighbor; need to maintain state of from which direction this node was reached
            var newNode = new AStarGridSearch.Node() { Position = (newX, newY), Parent = currentNode, State = dirIndex };
            adjacentNodes.Add(newNode);
        }

        return adjacentNodes;
    }


    protected override Map Parse(string input)
    {
        return new Map(input.Split("\n").Where(p => p != "").Select(l => l.ToCharArray()).ToArray());
    }
}