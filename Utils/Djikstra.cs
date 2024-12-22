namespace AOC.Utils;

// Djikstra's algorithm.

// extend the base node to store best weight, but allow callers to further extend it.
// like A*, it's a little weird that we are storing search state in the node itself.
// A cleaner idea would be to return something like a Result<T> which contains the path(s), lowest total weight, etc
// and leaves the nodes themselves unmodified.
public class DjikstraNode<T> : BaseNode<T> where T : DjikstraNode<T>
{
    public int BestWeight = int.MaxValue;
}

public class Djikstra<T> where T : DjikstraNode<T>
{
    public static List<List<T>> Search(T start, List<T> allNodes, Func<T, bool> IsEnd)
    {
        // unclear why we can't get rid of weights here and store the best weight in the node itself.
        // it works fine for Day16 but not Day21.
        // if this can be fixed then we don't need allNodes either.
        var weights = allNodes.Select(p => (Node: p, details: (Previous: (T?)null, Weight: int.MaxValue))).ToDictionary(p => p.Node, p => p.details);
        var queue = new PriorityQueue<T, int>();

        var allPaths = new List<List<T>>();

        var predecessors = new Dictionary<T, List<T>>();

        weights[start] = (null, 0);
        //start.BestWeight = 0;

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (IsEnd(current))
            {
                current.BestWeight = weights[current].Weight;
                return FindAllPaths(current, predecessors, start);
            }

            var currentWeight = weights[current].Weight;
            //var currentWeight = current.BestWeight;

            foreach (var edge in current.Edges)
            {
                //var weight = To.BestWeight;
                var weight = weights[edge.To].Weight;
                var newWeight = currentWeight + edge.Weight;

                if (newWeight < weight)
                {
                    weights[edge.To] = (current, newWeight);
                    //To.BestWeight = newWeight;
                    predecessors[edge.To] = [current];
                    queue.Remove(edge.To, out _, out _);           // .NET 9
                    queue.Enqueue(edge.To, newWeight);
                }
                else if (newWeight == weight)
                {
                    //if (!predecessors.ContainsKey(To)) predecessors[To] = [];
                    predecessors[edge.To].Add(current);            // save predeccessors for all paths that led to this same lowest weight
                }
            }
        }

        System.Console.WriteLine("Didn't find a path!");
        return allPaths;
    }

    // recursive backtrack to enumerate all paths that led to this same lowest weight.  credit to a random quora post.
    private static List<List<T>> FindAllPaths(T current, Dictionary<T, List<T>> predecessors, T start)
    {
        if (current == start)
            return [[current]];

        var paths = new List<List<T>>();

        foreach (var predecessor in predecessors[current])
        {
            foreach (var path in FindAllPaths(predecessor, predecessors, start))
            {
                paths.Add([.. path, current]);
            }
        }

        return paths;
    }
}
