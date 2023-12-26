namespace AOC.AOC2023;

public class Day25 : Day<Day25.Graph>
{
    protected override string? SampleRawInput { get => "jqt: rhn xhk nvd\nrsh: frs pzl lsr\nxhk: hfx\ncmg: qnr nvd lhk bvb\nrhn: xhk bvb hfx\nbvb: xhk hfx\npzl: lsr hfx nvd\nqnr: nvd\nntq: jqt hfx bvb xhk\nnvd: lhk\nlsr: lhk\nrzs: qnr cmg lsr rsh\nfrs: qnr lhk lsr"; }

    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();

        // breadth-first search to target node or the whole graph, counting navigated edges
        public static int BFS(Node start, Node? end = null)
        {
            var visited = new HashSet<Node>() { start };
            var queue = new Queue<Node>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node == end) break;

                foreach (var neighbor in node.Neighbors.Where(p => p.Active))
                {
                    var dest = neighbor.First != node ? neighbor.First : neighbor.Second;
                    if (!visited.Contains(dest))
                    {
                        visited.Add(dest);
                        queue.Enqueue(dest);
                        neighbor.NavigatedCount++;
                    }
                }
            }

            return visited.Count;
        }

        public void ResetEdges()
        {
            foreach (var edge in Edges)
            {
                edge.NavigatedCount = 0;
                edge.Active = true;
            }
        }
    }

    public class Node
    {
        public required string Name { get; set; }
        public List<Edge> Neighbors { get; set; } = new List<Edge>();
    }

    // same instance for a bidirectional edge, but in both nodes' Neighbors lists
    public class Edge
    {
        public required Node First { get; set; }
        public required Node Second { get; set; }
        public bool Active { get; set; } = true;
        public int NavigatedCount { get; set; }
    }

    protected override long Part1()
    {
        var rnd = new Random();
        int ct = 0;

        while (true)
        {
            Input.ResetEdges();
            // pick 1000 random start and end nodes, BFS between them, and pick the top 3 most-used "wires" to snip (these are the most likely to segment the graph when all snipped)
            for (var i=0; i<1000; i++)
            {
                var start = Input.Nodes.ElementAt(rnd.Next(Input.Nodes.Count));
                var end = Input.Nodes.Where(p => p != start).ElementAt(rnd.Next(Input.Nodes.Count-1));

                Graph.BFS(start, end);
            }

            // try snipping the top 3 wires and see if we segmented the graph.
            var topEdges = Input.Edges.OrderByDescending(p => p.NavigatedCount).Take(3).ToList();
            topEdges.ForEach(p => p.Active = false);

            ct = Graph.BFS(Input.Nodes.First());
            if (ct != Input.Nodes.Count) break;

            System.Console.WriteLine("No solution found, trying 1000 more random paths");
        }

        return (Input.Nodes.Count - ct) * ct;
    }

    protected override long Part2()
    {
        return 0;           // there is no part 2!
    }

    protected override Graph Parse(string input)
    {
        var nodes = new Dictionary<string, Node>();
        var edges = new List<Edge>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(": ");
            var name = parts[0];
            if (!nodes.ContainsKey(name))
            {
                nodes.Add(name, new Node() { Name = name });
            }
            var node = nodes[name];
            
            foreach (var neighbor in parts[1].Split(" "))
            {
                if (!nodes.ContainsKey(neighbor))
                {
                    nodes.Add(neighbor, new Node() { Name = neighbor });
                }
                var neighborNode = nodes[neighbor];

                if (node.Neighbors.Any(p => p.First.Name == neighbor || p.Second.Name == neighbor)) continue;       // edge already exists

                // bidirectional connection
                var edge = new Edge() { First = node, Second = neighborNode };
                node.Neighbors.Add(edge);
                neighborNode.Neighbors.Add(edge);
                edges.Add(edge);
            }
        }

        return new Graph() { Nodes = nodes.Values.ToList(), Edges = edges };
    }
}