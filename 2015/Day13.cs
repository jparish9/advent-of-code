using AOC.Utils;

namespace AOC.AOC2015;

public class Day13 : Day<Graph<NamedNode>>
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


    protected override Answer Part1()
    {
        return SearchGraph();
    }

    protected override Answer Part2()
    {
        // add self, linked to all others, with bidirectional weight of 0
        var self = new NamedNode() { Name = "Self"};
        foreach (var node in Input.Nodes)
        {
            node.AddEdge(self, 0);
            self.AddEdge(node, 0);
        }
        Input.Nodes.Add(self);

        return SearchGraph();
    }

    private int SearchGraph()
    {
        // any start node should yield the same result, since every complete path is a cycle of all nodes.
        // an edge is allowed if it is unvisited, plus a special case for the last edge back to the first node.
        // a path is complete if it is a complete cycle of all nodes (start == end).
        // edge weight needs to count bidirectional weight (A to B + B to A) when it is traversed.
        return Input.Search(
            start: Input.Nodes.First(),
            compare: Graph<NamedNode>.Maximize,
            edges: (currentNode, visited, path, nodeCt) => currentNode.Edges.Where(p => !(visited.Contains(p.To) && (visited.Count != nodeCt || p.To != path[0]))),
            pathComplete: (currentNode, end, path, nodeCt) => path.Count == nodeCt && currentNode == path[0],
            edgeWeight: (edge) => edge.Weight + edge.To.Edges.Single(p => p.To == edge.From).Weight
        );
    }

    protected override Graph<NamedNode> Parse(RawInput input)
    {
        var graph = new Graph<NamedNode>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" ");
            var from = parts[0];
            var to = parts[10].TrimEnd('.');
            var weight = int.Parse(parts[3]) * (parts[2] == "gain" ? 1 : -1);

            var fromNode = graph.Nodes.FirstOrDefault(n => n.Name == from);
            if (fromNode == null)
            {
                fromNode = new NamedNode() { Name = from };
                graph.Nodes.Add(fromNode);
            }

            var toNode = graph.Nodes.FirstOrDefault(n => n.Name == to);
            if (toNode == null)
            {
                toNode = new NamedNode() { Name = to };
                graph.Nodes.Add(toNode);
            }

            // edges are not bidirectional!  different weights from A to B as from B to A.  single edge per input line.
            fromNode.AddEdge(toNode, weight);
        }

        return graph;
    }
}