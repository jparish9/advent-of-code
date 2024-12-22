using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AOC.Utils;

namespace AOC.AOC2024;

public class Day16 : Day<Day16.Map>
{
    // small example
    //protected override string? SampleRawInput { get => "###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############"; }
    // larger example
    protected override string? SampleRawInput { get => "#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################";}
    // this edge case exposed a bug where reaching a location from two different directions was incorrectly considered the same node.
    //protected override string? SampleRawInput { get => "##########\n#.......E#\n#.##.#####\n#..#.....#\n##.#####.#\n#S.......#\n##########"; }

    private static readonly (int X, int Y)[] Directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];           // up, right, down, left

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
                node.AddEdge(nodeGrid[node.Y][node.X][(node.Facing + 1) % 4], 1000);
                node.AddEdge(nodeGrid[node.Y][node.X][(4 + (node.Facing - 1)) % 4], 1000);

                // check one step in the current facing direction
                var (xDir, yDir) = Directions[node.Facing];
                var newX = node.X + xDir;
                var newY = node.Y + yDir;

                if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;
                if (newY == Start.Y && newX == Start.X) continue;                        // don't revisit start
                if (Grid[newY][newX] == '#') continue;           // wall

                // allow move one step in facing direction
                node.AddEdge(nodeGrid[newY][newX][node.Facing], 1);
            }
        }

        public char[][] Grid;

        [NotNull]
        public Node Start;
        [NotNull]
        public (int X, int Y) End = (-1, -1);          // end check should allow for any facing; not a single "node" (x, y, facing) just a location (x, y)
        public List<Node> AllNodes = [];
        public List<List<Node>> AllPaths = [];           // cache for part 2
    }

    public class Node : DjikstraNode<Node>
    {
        public int X;
        public int Y;
        public int Facing;
    }

    protected override Answer Part1()
    {
        // djikstra's algorithm using Edges and starting at S (facing 1,0), ending at E (facing any); save all [best] paths for part 2
        Input.AllPaths = Djikstra<Node>.Search(Input.Start, Input.AllNodes, p => p.X == Input.End.X && p.Y == Input.End.Y);
        return Input.AllPaths.First().Last().BestWeight;
    }

    protected override Answer Part2()
    {
        return Input.AllPaths.SelectMany(p => p).Select(p => (p.X, p.Y)).Distinct().Count();
    }

    protected override Map Parse(string input)
    {
        return new Map(input.Split("\n").Where(p => p != "").Select(l => l.ToCharArray()).ToArray());
    }
}