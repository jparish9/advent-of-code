namespace AOC.Utils;

// weighted graph, with highly configurable search
public class Graph
{
    public List<Node> Nodes { get; set; } = new List<Node>();

    // static functions defining default search behavior
    public static readonly Func<int, int, bool> Minimize = (a, b) => a < b;
    public static readonly Func<int, int, bool> Maximize = (a, b) => a > b;
    public static readonly Func<Node, HashSet<Node>, List<Node>, int, IEnumerable<Edge>> NotVisited = (currentNode, visited, path, nodeCt) => currentNode.Edges.Where(p => !visited.Contains(p.To));
    public static readonly Func<Node?, List<Node>, int, bool> AllNodesVisitedOrEndReached = (endNode, path, nodeCt) => path.Count == nodeCt || path[^1] == endNode;
    public static readonly Func<Edge, int> EdgeWeight = (edge) => edge.Weight;

    // default search is to minimize total weight over any path that visits every node exactly once
    // override parameter(s) to change this.
    public int Search(Node? start = null, Node? end = null,
        int? startValue = null, Func<int, int, bool>? compare = null,
        Func<Node, HashSet<Node>, List<Node>, int, IEnumerable<Edge>>? edges = null,
        Func<Node?, List<Node>, int, bool>? pathComplete = null,
        Func<Edge, int>? edgeWeight = null)
    {
        compare ??= Minimize;
        startValue ??= compare == Minimize ? int.MaxValue : int.MinValue;
        var best = startValue!.Value;

        var startNodes = start == null ? Nodes : new List<Node>() { start };

        foreach (var node in startNodes)
        {
            var path = new List<Node>();
            var visited = new HashSet<Node>();

            Search(node, end, visited, path, 0, ref best, compare, edges ?? NotVisited, pathComplete ?? AllNodesVisitedOrEndReached, edgeWeight ?? EdgeWeight);
        }

        return best;
    }

    private void Search(Node current, Node? end, HashSet<Node> visited, List<Node> path,
        int weight, ref int best, Func<int, int, bool> compare,
        Func<Node, HashSet<Node>, List<Node>, int, IEnumerable<Edge>> edges,
        Func<Node?, List<Node>, int, bool> pathComplete,
        Func<Edge, int> edgeWeight)
    {
        var thisVisited = new HashSet<Node>(visited) { current };
        var thisPath = new List<Node>(path) { current };

        if (pathComplete(end, thisPath, Nodes.Count))
        {
            if (compare(weight, best))
            {
                best = weight;
                //System.Console.WriteLine("Found a better path with weight " + weight + ": " + string.Join(" -> ", thisPath.Select(p => p.Name)));
            }
            return;
        }

        foreach (var edge in edges(current, thisVisited, thisPath, Nodes.Count))
        {
            //System.Console.WriteLine("Visiting " + edge.To.Name + " from " + edge.From.Name + "(current node " + current.Name + ") with weight " + edgeWeight(edge));
            Search(edge.To, end, thisVisited, thisPath, weight + edgeWeight(edge), ref best, compare, edges, pathComplete, edgeWeight);        // unvisited; recurse
        }
    }
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