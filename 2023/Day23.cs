using System.Diagnostics.CodeAnalysis;

namespace AOC.AOC2023;

public class Day23 : Day<Day23.TrailMap>
{
    protected override string? SampleRawInput { get => "#.#####################\n#.......#########...###\n#######.#########.#.###\n###.....#.>.>.###.#.###\n###v#####.#v#.###.#.###\n###.>...#.#.#.....#...#\n###v###.#.#.#########.#\n###...#.#.#.......#...#\n#####.#.#.#######.#.###\n#.....#.#.#.......#...#\n#.#####.#.#.#########v#\n#.#...#...#...###...>.#\n#.#.#v#######v###.###v#\n#...#.>.#...>.>.#.###.#\n#####v#.#.###v#.#.###.#\n#.....#...#...#.#.#...#\n#.#########.###.#.#.###\n#...###...#...#...#.###\n###.###.#.###v#####v###\n#...#...#.#.>.>.#.>.###\n#.###.###.#.###.#.#v###\n#.....###...###...#...#\n#####################.#"; }

    protected override bool Part2ParsedDifferently => true;         // parse v and > as . for part 2 (slopes are passable)

    public class TrailMap
    {
        [SetsRequiredMembers]
        public TrailMap(char[][] map)
        {
            Map = map;
        }

        public required char[][] Map { get; set; }

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

    // simple weighted graph implementation for two-dimensional graph vertices; probably factor out at some point, make generic, maybe add general graph search algos
    private class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
    }

    private class Node
    {
        public (int X, int Y) Position { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }

    private class Edge
    {
        public required Node To { get; set; }
        public (int X, int Y) Direction { get; set; }
        public int Weight { get; set; }
    }

    protected override long Part1()
    {
        return GetLongestPath();
    }

    protected override long Part2()
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

        var graph = new Graph();
        var start = Input.FindStart();
        var visited = new HashSet<(int X, int Y)>() { start };
        var startNode = new Node() { Position = start };
        graph.Nodes.Add(startNode);

        BuildGraph(graph, startNode, Input.FindEnd(), null, visited);

        return FindHighestWeightPath(graph);
    }

    // build a graph of just the critical nodes (forks, start, end) in the map, with edge weights being the length of the "roads" connecting them.
    private void BuildGraph(Graph graph, Node currentNode, (int X, int Y) endPos, (int X, int Y)? direction, HashSet<(int X, int Y)> visited)
    {
        // make a copy of visited, this is this path's visited set, not a global one (we don't want to modify the caller's visited set)
        var newVisited = new HashSet<(int X, int Y)>(visited);

        var adjacentNodes = new List<(int X, int Y)>();

        var allDirs = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var dirs = direction != null ? new[] { ((int, int))direction } : allDirs;       // when called recursively, our single initial direction (from the fork node) is given.
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

        if (!adjacentNodes.Any()) return;           // dead end

        if (adjacentNodes[0] == endPos)            // reached the target
        {
            // we may have reached the target in another path, don't duplicate the end node
            var endNode = graph.Nodes.FirstOrDefault(p => p.Position == adjacentNodes[0]);
            if (endNode == null)
            {
                endNode = new Node() { Position = adjacentNodes[0] };
                graph.Nodes.Add(endNode);
            }

            // currentNode may already have the end node as an edge, don't duplicate that either.
            if (!currentNode.Edges.Any(p => p.To == endNode))
                currentNode.Edges.Add(new Edge() { To = endNode, Weight = weight, Direction = firstDir!.Value });
            return;
        }

        // reached a fork (two or more adjacent nodes)
        // (x, y) is the fork node, adjacentNodes are the paths we can take from it.
        // don't duplicate the fork node.
        var forkNode = graph.Nodes.FirstOrDefault(p => p.Position == (x, y));
        if (forkNode == null)
        {
            forkNode = new Node() { Position = (x, y) };
            graph.Nodes.Add(forkNode);
        }
        
        // don't duplicate the edge from currentNode to forkNode
        if (!currentNode.Edges.Any(p => p.To == forkNode))
            currentNode.Edges.Add(new Edge() { To = forkNode, Weight = weight, Direction = firstDir!.Value });

        foreach (var (adjX, adjY) in adjacentNodes)
        {
            var adjacentDir = (adjX - x, adjY - y);
            if (forkNode.Edges.Any(p => p.Direction == adjacentDir)) continue;          // we've already been in this direction from this node

            // recursive call for each unexplored fork path
            BuildGraph(graph, forkNode, endPos, adjacentDir, newVisited);
        }
    }

    // exhaustive search of the graph to find the path from start to end with highest weight
    private int FindHighestWeightPath(Graph graph)
    {
        var start = graph.Nodes.First(p => p.Position == Input.FindStart());
        var end = graph.Nodes.First(p => p.Position == Input.FindEnd());

        var visited = new HashSet<Node>();
        var path = new List<Node>();

        var highestWeight = 0;

        FindHighestWeightPath(start, end, visited, path, 0, ref highestWeight);

        return highestWeight;
    }

    private void FindHighestWeightPath(Node current, Node end, HashSet<Node> visited, List<Node> path, int weight, ref int highestWeight)
    {
        if (current == end)
        {
            if (weight > highestWeight) highestWeight = weight;
            return;
        }

        visited.Add(current);
        path.Add(current);

        foreach (var edge in current.Edges)
        {
            if (visited.Contains(edge.To)) continue;

            FindHighestWeightPath(edge.To, end, visited, path, weight + edge.Weight, ref highestWeight);
        }

        visited.Remove(current);
        path.Remove(current);
    }

    protected override TrailMap Parse(string input)
    {
        var lines = input.Split("\n").Where(p => p != "").ToArray();
        var map = new char[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            // for part 2, slopes are passable.
            map[i] = (IsPart2 ? lines[i].Replace(">", ".").Replace("v", ".") : lines[i]).ToCharArray();
        }

        return new TrailMap(map);
    }
}