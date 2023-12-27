namespace AOC.AOC2015;

public class Day13 : Day<Day13.Graph>
{
    protected override string? SampleRawInput { get => "Alice would gain 54 happiness units by sitting next to Bob.\n" +
        "Alice would lose 79 happiness units by sitting next to Carol.\n" +
        "Alice would lose 2 happiness units by sitting next to David.\n" +
        "Bob would gain 83 happiness units by sitting next to Alice.\n" +
        "Bob would lose 7 happiness units by sitting next to Carol.\n" +
        "Bob would lose 63 happiness units by sitting next to David.\n" +
        "Carol would lose 62 happiness units by sitting next to Alice.\n" +
        "Carol would gain 60 happiness units by sitting next to Bob.\n" +
        "Carol would gain 55 happiness units by sitting next to David.\n" +
        "David would gain 46 happiness units by sitting next to Alice.\n" +
        "David would lose 7 happiness units by sitting next to Bob.\n" +
        "David would gain 41 happiness units by sitting next to Carol."; }

    // yet another weighted graph!
    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
    }

    public class Node
    {
        public required string Name { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }

    public class Edge
    {
        public required Node From { get; set; }
        public required Node To { get; set; }
        public int Weight { get; set; }
    }

    protected override long Part1()
    {
        return FindPath(Input, int.MinValue, (a, b) => a > b);
    }

    protected override long Part2()
    {
        var self = new Node() { Name = "Self" };
        foreach (var node in Input.Nodes)
        {
            node.Edges.Add(new Edge() { From = node, To = self, Weight = 0 });
            self.Edges.Add(new Edge() { From = self, To = node, Weight = 0 });
        }
        Input.Nodes.Add(self);

        return FindPath(Input, int.MinValue, (a, b) => a > b);
    }

    // search from a start node back to itself (hitting every node exactly once), keeping track of the "best" path length according to the given comparison function
    private int FindPath(Graph graph, int startValue, Func<int, int, bool> compare)
    {
        var best = startValue;

        // start node shouldn't matter since every complete path is a cycle of every node
        var path = new List<Node>();
        var visited = new HashSet<Node>();

        FindPath(graph.Nodes.First(), visited, path, 0, ref best, compare);

        return best;
    }

    private void FindPath(Node current, HashSet<Node> visited, List<Node> path, int dist, ref int best, Func<int, int, bool> compare)
    {
        var thisVisited = new HashSet<Node>(visited) { current };
        var thisPath = new List<Node>(path) { current };

        if (thisPath.Count == Input.Nodes.Count + 1)         // must start and end at the same node
        {
            if (compare(dist, best))
            {
                best = dist;
                //System.Console.WriteLine("Found a better path with distance " + dist + ": " + string.Join(" -> ", thisPath.Select(p => p.Name)));
            }
            return;
        }

        foreach (var edge in current.Edges)
        {
            if (thisVisited.Contains(edge.To) && (thisVisited.Count != Input.Nodes.Count || edge.To != thisPath[0])) continue;     // already visited, except if we are at the last node and can go back to the start

            // edge weight is bidirectional!  A is now next to B, but that means B is also next to A
            FindPath(edge.To, thisVisited, thisPath, dist + edge.Weight + edge.To.Edges.Single(p => p.To == current).Weight, ref best, compare);        // unvisited; recurse
        }
    }

    protected override Graph Parse(string input)
    {
        var graph = new Graph();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(" ");
            var from = parts[0];
            var to = parts[10].TrimEnd('.');
            var weight = int.Parse(parts[3]) * (parts[2] == "gain" ? 1 : -1);

            var fromNode = graph.Nodes.FirstOrDefault(n => n.Name == from);
            if (fromNode == null)
            {
                fromNode = new Node() { Name = from };
                graph.Nodes.Add(fromNode);
            }

            var toNode = graph.Nodes.FirstOrDefault(n => n.Name == to);
            if (toNode == null)
            {
                toNode = new Node() { Name = to };
                graph.Nodes.Add(toNode);
            }

            // edges are not bidirectional!  different weights from A to B as from B to A.  single edge per input line.
            var edge = new Edge() { From = fromNode, To = toNode, Weight = weight };
            fromNode.Edges.Add(edge);
        }

        return graph;
    }
}