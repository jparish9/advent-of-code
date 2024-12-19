using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AOC.AOC2024;

public class Day16 : Day<Day16.Map>
{
    // small example
    //protected override string? SampleRawInput { get => "###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############"; }
    // larger example
    protected override string? SampleRawInput { get => "#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################";}
    // this edge case exposed a bug where reaching a location from two different directions was incorrectly considered the same node.
    //protected override string? SampleRawInput { get => "##########\n#.......E#\n#.##.#####\n#..#.....#\n##.#####.#\n#S.......#\n##########"; }

    private static readonly (int X, int Y)[] Directions = new[] { (0, -1), (1, 0), (0, 1), (-1, 0) };           // up, right, down, left

    public class Map
    {
        public Map(char[][] Grid)
        {
            var sw = new Stopwatch();
            sw.Start();
            this.Grid = Grid;

            var nodeGrid = new Node[Grid.Length][][];           // for fast access when building the edge map

            for (var y=0; y<Grid.Length; y++)
            {
                nodeGrid[y] = new Node[Grid[y].Length][];
                for (var x=0; x<Grid[y].Length; x++)
                {
                    if (Grid[y][x] == '#') continue;
                    if (Grid[y][x] == 'E') End = (x, y);

                    nodeGrid[y][x] = new Node[Directions.Length];

                    for (var facingNdx=0; facingNdx<Directions.Length; facingNdx++)
                    {
                        var node = new Node() { X = x, Y = y, Facing = facingNdx };
                        AllNodes.Add(node);
                        nodeGrid[y][x][facingNdx] = node;

                        if (Grid[y][x] == 'S' && facingNdx == 1) Start = node;
                    }
                }
            }

            if (Start == null || End == (-1, -1)) throw new Exception("Start or end not found");

            foreach (var node in AllNodes)
            {
                // allow turning left or right
                node.Edges.Add(new Edge() { ConnectedTo = nodeGrid[node.Y][node.X][(node.Facing + 1) % 4], Weight = 1000 });
                node.Edges.Add(new Edge() { ConnectedTo = nodeGrid[node.Y][node.X][(4 + (node.Facing - 1)) % 4], Weight = 1000 });

                // check one step in the current facing direction
                var (xDir, yDir) = Directions[node.Facing];
                var newX = node.X + xDir;
                var newY = node.Y + yDir;

                if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;
                if (newY == Start.Y && newX == Start.X) continue;                        // don't revisit start
                if (Grid[newY][newX] == '#') continue;           // wall

                // allow move one step in facing direction
                node.Edges.Add(new Edge() { ConnectedTo = nodeGrid[newY][newX][node.Facing], Weight = 1 });
            }
        }

        public char[][] Grid;

        [NotNull]
        public Node Start;
        [NotNull]
        public (int X, int Y) End = (-1, -1);          // end check should allow for any facing; not a single "node" (x, y, facing) just a location (x, y)
        public List<Node> AllNodes = new();

        public List<List<Node>> AllPaths = new();           // cache for part 2
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
        // djikstra's algorithm using Edges and starting at S (facing 1,0), ending at E (facing any); save extra predecessors map so we can backtrack and get ALL paths with the same weight for part 2.
        // might refactor this for reuse at some point.  the "saving all shortest paths" part seems useful.
        var start = Input.Start;

        var weights = Input.AllNodes.Select(p => (Node: p, details: (Previous: (Node?)null, Weight: int.MaxValue))).ToDictionary(p => p.Node, p => p.details);
        var queue = new PriorityQueue<Node, int>();

        var predecessors = new Dictionary<Node, List<Node>>();

        weights[start] = (null, 0);

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.X == Input.End.X && current.Y == Input.End.Y)
            {
                // cache for part 2
                Input.AllPaths = FindAllPaths(current, predecessors, start);
                return weights[current].Weight;
            }

            var currentWeight = weights[current].Weight;

            foreach (var edge in current.Edges)
            {
                var weight = weights[edge.ConnectedTo].Weight;
                var newWeight = currentWeight + edge.Weight;

                if (newWeight < weight)
                {
                    weights[edge.ConnectedTo] = (current, newWeight);
                    predecessors[edge.ConnectedTo] = new List<Node> { current };
                    queue.Remove(edge.ConnectedTo, out _, out _);           // .NET 9
                    queue.Enqueue(edge.ConnectedTo, newWeight);
                }
                else if (newWeight == weight)
                {
                    predecessors[edge.ConnectedTo].Add(current);            // save equivalent path (for part 2)
                }
            }
        }

        System.Console.WriteLine("Didn't find a path!");
        return 0;
    }

    protected override Answer Part2()
    {
        return Input.AllPaths.SelectMany(p => p).Select(p => (p.X, p.Y)).Distinct().Count();
    }


    // recursive backtrack to enumerate all paths that led to this same lowest weight.  credit to a random quora post.
    private List<List<Node>> FindAllPaths(Node current, Dictionary<Node, List<Node>> predecessors, Node start)
    {
        if (current == start)
            return new List<List<Node>> { new() { current } };

        var paths = new List<List<Node>>();

        foreach (var predecessor in predecessors[current])
        {
            foreach (var path in FindAllPaths(predecessor, predecessors, start))
            {
                paths.Add(path.Append(current).ToList());
            }
        }

        return paths;
    }

    protected override Map Parse(string input)
    {
        return new Map(input.Split("\n").Where(p => p != "").Select(l => l.ToCharArray()).ToArray());
    }
}