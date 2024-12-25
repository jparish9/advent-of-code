using System.Diagnostics.CodeAnalysis;
using AOC.Utils;

namespace AOC.AOC2023;

public class Day23 : Day<Day23.TrailMap>
{
    protected override string? SampleRawInput { get => "#.#####################\n#.......#########...###\n#######.#########.#.###\n###.....#.>.>.###.#.###\n###v#####.#v#.###.#.###\n###.>...#.#.#.....#...#\n###v###.#.#.#########.#\n###...#.#.#.......#...#\n#####.#.#.#######.#.###\n#.....#.#.#.......#...#\n#.#####.#.#.#########v#\n#.#...#...#...###...>.#\n#.#.#v#######v###.###v#\n#...#.>.#...>.>.#.###.#\n#####v#.#.###v#.#.###.#\n#.....#...#...#.#.#...#\n#.#########.###.#.#.###\n#...###...#...#...#.###\n###.###.#.###v#####v###\n#...#...#.#.>.>.#.>.###\n#.###.###.#.###.#.#v###\n#.....###...###...#...#\n#####################.#"; }

    [method: SetsRequiredMembers]
    public class TrailMap(char[][] map)
    {
        public required char[][] Map { get; set; } = map;

        public (int X, int Y) FindStart()
        {
            // start is single open tile (.) on top row
            return (FindDot(Map[0]), 0);
        }

        public (int X, int Y) FindEnd()
        {
            // end is single open tile (.) on bottom row
            return (FindDot(Map[^1]), Map.Length - 1);
        }

        private static int FindDot(char[] row)
        {
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == '.') return i;
            }

            throw new Exception("Not found!");
        }
    }

    // extend the base Node class to include position and edges that include direction
    private class TrailMapNode : BaseNode<TrailMapNode>
    {
        public (int X, int Y) Position { get; set; }

        // add an edge extended with direction
        public void AddEdge(TrailMapNode to, int weight, (int X, int Y) direction)
        {
            Edges.Add(new TrailMapEdge() { From = this, To = to, Weight = weight, Direction = direction });
        }
    }

    // extend the base Edge class to include direction
    private class TrailMapEdge : Edge<TrailMapNode>
    {
        public (int X, int Y) Direction { get; set; }
    }

    protected override Answer Part1()
    {
        return GetLongestPath();
    }

    protected override Answer Part2()
    {
        return GetLongestPath();
    }

    private int GetLongestPath()
    {
        // the "longest path" problem is, in the general case, "strongly NP-hard" (meaning there is almost certainly no fast exact OR approximate algorithm).
        // however, while the paths are quite long here, the number of critical nodes (those with more than one successor, i.e. forks we could take in the path) is quite small.
        // so we can condense the problem down to just a graph (not necessarily a DAG, need to handle cycles) of the critical nodes,
        // where the edge weights are the lengths of the "roads" connecting them, and then probably just brute-force search the highest-weight path from start to end on the (much) smaller graph.
        // this is still just a (much) smaller NP-hard problem, but should be reduced enough that it can be brute-forced in a reasonable amount of time.

        var graph = new Graph<TrailMapNode>();
        var start = Input.FindStart();
        var visited = new HashSet<(int X, int Y)>() { start };
        var startNode = new TrailMapNode() { Position = start };
        graph.Nodes.Add(startNode);
        BuildGraph(graph, startNode, Input.FindEnd(), null, visited);
        var endNode = graph.Nodes.First(p => p.Position == Input.FindEnd());

        return graph.Search(
            start: startNode,
            end: endNode,
            compare: Graph<NamedNode>.Maximize
        );
    }

    // build a graph of just the critical nodes (forks, start, end) in the map, with edge weights being the length of the "roads" connecting them.
    private void BuildGraph(Graph<TrailMapNode> graph, TrailMapNode currentNode, (int X, int Y) endPos, (int X, int Y)? direction, HashSet<(int X, int Y)> visited)
    {
        // make a copy of visited, this is this path's visited set, not a global one (we don't want to modify the caller's visited set)
        var newVisited = new HashSet<(int X, int Y)>(visited);

        var adjacentNodes = new List<(int X, int Y)>();

        var allDirs = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var dirs = direction != null ? [((int, int))direction] : allDirs;       // when called recursively, our single initial direction (from the fork node) is given.
        var firstDir = direction;
        
        var x = currentNode.Position.X;
        var y = currentNode.Position.Y;

        var weight = 0;

        do
        {
            adjacentNodes.Clear();
            foreach (var (xDir, yDir) in dirs)
            {
                var newX = x + xDir;
                var newY = y + yDir;

                if (newX < 0 || newY < 0 || newY >= Input.Map.Length || newX >= Input.Map[0].Length) continue;           // out of bounds
                if (newVisited.Contains((newX, newY))) continue;                                                         // already visited
                if (Input.Map[newY][newX] == '#') continue;                                                              // forest (impassable)

                if ((Input.Map[y][x] == '>' && xDir != 1)
                    || (Input.Map[y][x] == 'v' && yDir != 1)) continue;                                                  // must continue down-slope

                if ((Input.Map[newY][newX] == '>' && xDir == -1)
                    || (Input.Map[newY][newX] == 'v' && yDir == -1)) continue;                                           // can't go up-slope
                
                adjacentNodes.Add((newX, newY));        // valid neighbor

                firstDir ??= (xDir, yDir);              // keep track of the first direction we went on this "road" so we can save it when creating the edge to the next node
            }

            if (adjacentNodes.Count == 1)               // on a "road"; count visited, increment weight (step count), and continue
            {
                x = adjacentNodes[0].X;
                y = adjacentNodes[0].Y;
                weight++;
                newVisited.Add(adjacentNodes[0]);
            }

            dirs = allDirs;         // if there was a given initial direction, once we've taken the first step, we can go in any direction
        }
        while (adjacentNodes.Count == 1 && (adjacentNodes[0] != endPos));           // iterate until we hit a fork, a dead end, or the target

        if (adjacentNodes.Count == 0) return;           // dead end

        if (adjacentNodes[0] == endPos)            // reached the target
        {
            // we may have reached the target in another path, don't duplicate the end node
            var endNode = graph.Nodes.FirstOrDefault(p => p.Position == adjacentNodes[0]);
            if (endNode == null)
            {
                endNode = new TrailMapNode() { Position = adjacentNodes[0] };
                graph.Nodes.Add(endNode);
            }

            // currentNode may already have the end node as an edge, don't duplicate that either.
            if (!currentNode.Edges.Any(p => p.To == endNode))
                currentNode.AddEdge(endNode, weight, firstDir!.Value);
                //currentNode.Edges.Add(new Day23Edge<Day23Node>() { To = endNode, Weight = weight, Direction = firstDir!.Value });
            return;
        }

        // reached a fork (two or more adjacent nodes)
        // (x, y) is the fork node, adjacentNodes are the paths we can take from it.
        // don't duplicate the fork node.
        var forkNode = graph.Nodes.FirstOrDefault(p => p.Position == (x, y));
        if (forkNode == null)
        {
            forkNode = new TrailMapNode() { Position = (x, y) };
            graph.Nodes.Add(forkNode);
        }
        
        // don't duplicate the edge from currentNode to forkNode
        if (!currentNode.Edges.Any(p => p.To == forkNode))
            currentNode.AddEdge(forkNode, weight, firstDir!.Value);
            //currentNode.Edges.Add(new Day23Edge() { To = forkNode, Weight = weight, Direction = firstDir!.Value });

        foreach (var (adjX, adjY) in adjacentNodes)
        {
            var adjacentDir = (adjX - x, adjY - y);
            if (forkNode.Edges.Any(p => ((TrailMapEdge)p).Direction == adjacentDir)) continue;          // we've already been in this direction from this node

            // recursive call for each unexplored fork path
            BuildGraph(graph, forkNode, endPos, adjacentDir, newVisited);
        }
    }

    protected override TrailMap Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();
        var map = new char[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            // for part 2, slopes are passable.
            map[i] = (IsPart2 ? lines[i].Replace(">", ".").Replace("v", ".") : lines[i]).ToCharArray();
        }

        return new TrailMap(map);
    }
}