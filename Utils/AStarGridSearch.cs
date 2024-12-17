namespace AOC.Utils;

public class AStarGridSearch
{
    public class Node
    {
        public int F { get; set; }                  // estimated cost = actual cost + heuristic cost
        public int G { get; set; }                  // actual cost
        public int H { get; set; }                  // heuristic cost
        public (int X, int Y) Position { get; set; }    // (x, y)
        public int State { get; set; }              // additional state for this node; can be used by GetNeighbors, and is referenced by EndStateCheck to determine if the end location is valid
        public Node? Parent { get; set; }

        // node is unique (for purposes of A* search) if position and state are the same
        public int HashCode { get => Position.GetHashCode() ^ State.GetHashCode(); }
    }

    // for ordering by F-values.  if F is equal, compare hashcodes to ensure uniqueness.
    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node? x, Node? y)
        {
            if (x == null && y == null) return 0;
            else if (x != null && y == null) return -1;
            else if (x == null && y != null) return 1;

            var f = x!.F.CompareTo(y!.F);
            if (f != 0) return f;

            return x.HashCode.CompareTo(y.HashCode);
        }
    }

    public static int ManhattanDistance(Node thisNode, (int X, int Y) endNode)
    {
        return Math.Abs(thisNode.Position.X - endNode.X) + Math.Abs(endNode.Y - endNode.Y);
    }

    public static List<Node> Search(
        Func<(int, int)> StartLocation,                 // return (single) starting location (x,y)
        Func<List<(int, int)>> EndLocations,            // return list of ending locations (x,y)
        Func<int, bool> EndStateCheck,                  // in addition to reaching one of EndLocations, this function is called to check if the end state is valid.  just return true if not needed.
        Func<Node, Node, int> Cost,                     // cost function, taking current node, target node, and returning the cost to move there
        Func<Node, List<Node>> Neighbors,               // return list of valid neighbors, given the current Node
        Func<Node, (int, int), int> Heuristic = null!   // optional heuristic cost function (currentNode, (end X, end Y)), will default to ManhattanDistance if not provided
    )
    {
        var startNode = new Node() { Position = StartLocation() };
        var endNodes = new HashSet<(int X, int Y)>() {};
        var endLocations = EndLocations();
        foreach (var end in endLocations)
        {
            endNodes.Add(end);
        }

        // maintain the open list both as a hash for O(1) lookup by position+state, and as a sorted set to quickly get the node with the lowest F score
        var open = new Dictionary<int, Node>();
        var openF = new SortedSet<Node>(new NodeComparer());

        // for closed, we don't need the node itself, just its hash to keep track of them.
        var closed = new HashSet<int>();

        var path = new List<Node>();              // to quiet null warning; empty path will be returned if no path found

        open.Add(startNode.HashCode, startNode);
        openF.Add(startNode);

        while (open.Any())
        {
            // get node with lowest F score
            var currentNode = openF.First();
            var currentHash = currentNode.HashCode;

            // remove from open list, add to closed list
            open.Remove(currentHash);
            openF.Remove(currentNode);
            closed.Add(currentHash);

            // if end is reached and valid, return completed path
            if (endNodes.Contains(currentNode.Position) && EndStateCheck(currentNode.State))
            {
                var node = currentNode;
                while (node != null)
                {
                    path.Add(node);
                    node = node.Parent;
                }

                path.Reverse();
                return path;
            }

            // get all adjacent nodes
            var adjacentNodes = new List<Node>();
            adjacentNodes.AddRange(Neighbors(currentNode));

            // loop through adjacent nodes
            foreach (var adjacentNode in adjacentNodes)
            {
                var hash = adjacentNode.HashCode;
                if (closed.Contains(hash)) continue;

                adjacentNode.G = currentNode!.G + Cost(currentNode!, adjacentNode);
                // H (heuristic cost) is manhattan distance to end.  if multiple ends, use the closest one.
                adjacentNode.H = endNodes.Min(p => Heuristic != null ? Heuristic(adjacentNode, p) : ManhattanDistance(adjacentNode, p));
                adjacentNode.F = adjacentNode.G + adjacentNode.H;

                if (open.ContainsKey(hash) && open[hash].G <= adjacentNode.G) continue;         // already in open list with lower G score

                if (open.ContainsKey(hash)) open[hash] = adjacentNode;                  // in list with higher G score, replace it
                else open.Add(hash, adjacentNode);                                      // not in list, add it

                openF.Add(adjacentNode);
            }
        }

        Console.WriteLine("did not find a path!");
        return path;
    }
}