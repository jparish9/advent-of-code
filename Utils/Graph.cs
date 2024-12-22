namespace AOC.Utils;

// weighted graph, with highly configurable search
public class Graph<T> where T : BaseNode<T>
{
    public List<T> Nodes { get; set; } = [];

    // static functions defining default search behavior
    public static readonly Func<int, int, bool> Minimize = (a, b) => a < b;
    public static readonly Func<int, int, bool> Maximize = (a, b) => a > b;
    public static readonly Func<T, HashSet<T>, List<T>, int, IEnumerable<Edge<T>>> NotVisited = (currentNode, visited, path, nodeCt) => currentNode.Edges.Where(p => !visited.Contains(p.To));
    public static readonly Func<T, T?, List<T>, int, bool> AllNodesVisitedOrEndReached = (currentNode, endNode, pathUpTo, nodeCt) => currentNode == endNode || pathUpTo.Count == nodeCt-1;
    public static readonly Func<Edge<T>, int> EdgeWeight = (edge) => edge.Weight;

    // default search is to minimize total weight over any path that visits every node exactly once
    // override parameter(s) to change this.
    public int Search(
        T? start = null,
        T? end = null,
        int? startValue = null,
        Func<int, int, bool>? compare = null,
        Func<T, HashSet<T>, List<T>, int, IEnumerable<Edge<T>>>? edges = null,
        Func<T, T?, List<T>, int, bool>? pathComplete = null,
        Func<Edge<T>, int>? edgeWeight = null)
    {
        compare ??= Minimize;
        startValue ??= compare == Minimize ? int.MaxValue : int.MinValue;
        var best = startValue!.Value;

        var startNodes = start == null ? Nodes : [start];

        foreach (var node in startNodes)
        {
            var path = new List<T>();
            var visited = new HashSet<T>();

            Search(node, end, visited, path, 0, ref best, compare, edges ?? NotVisited, pathComplete ?? AllNodesVisitedOrEndReached, edgeWeight ?? EdgeWeight);
        }

        return best;
    }

    private void Search(T current, T? end, HashSet<T> visited, List<T> path, int weight, ref int best,
        Func<int, int, bool> compare, Func<T, HashSet<T>, List<T>, int, IEnumerable<Edge<T>>> edges,
        Func<T, T?, List<T>, int, bool> pathComplete, Func<Edge<T>, int> edgeWeight)
    {
        if (pathComplete(current, end, path, Nodes.Count))
        {
            if (compare(weight, best))
            {
                best = weight;
                //System.Console.WriteLine("Found a better path with weight " + weight + ": " + string.Join(" -> ", thisPath.Select(p => p.Name)));
            }
            return;
        }

        visited.Add(current);
        path.Add(current);

        foreach (var edge in edges(current, visited, path, Nodes.Count))
        {
            Search(edge.To, end, visited, path, weight + edgeWeight(edge), ref best, compare, edges, pathComplete, edgeWeight);        // unvisited; recurse
        }

        visited.Remove(current);
        path.Remove(current);
    }
}
