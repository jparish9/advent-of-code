namespace AOC.AOC2015;

public class Day9 : Day<Day9.Graph>
{
    protected override string? SampleRawInput { get => "London to Dublin = 464\nLondon to Belfast = 518\nDublin to Belfast = 141"; }

    // weighted graph really needs to be factored out at this point...
    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
    }

    public class Node
    {
        public required string City { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }

    public class Edge
    {
        public required Node From { get; set; }
        public required Node To { get; set; }
        public int Distance { get; set; }
    }

    protected override long Part1()
    {
        return FindPath(Input, int.MaxValue, (a, b) => a < b);
    }

    protected override long Part2()
    {
        return FindPath(Input, int.MinValue, (a, b) => a > b);
    }

    // search from a start node without an endpoint, keeping track of the "best" path length according to the given comparison function
    private int FindPath(Graph graph, int startValue, Func<int, int, bool> compare)
    {
        var best = startValue;

        foreach (var start in graph.Nodes)
        {
            var path = new List<Node>();
            var visited = new HashSet<Node>();

            FindPath(start, visited, path, 0, ref best, compare);
        }

        return best;
    }

    private void FindPath(Node current, HashSet<Node> visited, List<Node> path, int dist, ref int best, Func<int, int, bool> compare)
    {
        var thisVisited = new HashSet<Node>(visited) { current };
        var thisPath = new List<Node>(path) { current };

        if (thisVisited.Count == Input.Nodes.Count)
        {
            if (compare(dist, best)) best = dist;
            return;
        }

        foreach (var edge in current.Edges)
        {
            if (thisVisited.Contains(edge.To)) continue;

            FindPath(edge.To, thisVisited, thisPath, dist + edge.Distance, ref best, compare);
        }
    }

    protected override Graph Parse(string input)
    {
        var graph = new Graph();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(" ");
            var from = parts[0];
            var to = parts[2];
            var distance = int.Parse(parts[4]);

            var fromNode = graph.Nodes.FirstOrDefault(p => p.City == from);
            if (fromNode == null)
            {
                fromNode = new Node() { City = from };
                graph.Nodes.Add(fromNode);
            }

            var toNode = graph.Nodes.FirstOrDefault(p => p.City == to);
            if (toNode == null)
            {
                toNode = new Node() { City = to };
                graph.Nodes.Add(toNode);
            }

            fromNode.Edges.Add(new Edge() { From = fromNode, To = toNode, Distance = distance });
            toNode.Edges.Add(new Edge() { From = toNode, To = fromNode, Distance = distance });
        }

        return graph;
    }
}