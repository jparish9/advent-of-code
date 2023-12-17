namespace AOC.AOC2023;

public class Day17 : Day<Day17.Map>
{
    protected override string? SampleRawInput { get => "2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533"; }

    //protected override string? SampleRawInput { get => "111111111111\n999999999991\n999999999991\n999999999991\n999999999991"; }

    public class Map
    {
        public required int[][] Grid { get; set; }

        // node class for A* search
        public class Node
        {
            public int F { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public (int, int) Position { get; set; }
            public int State { get; set; }          // memory of direction of last 3 steps taken; bitmask (0-3 left, 4-15 right, 16-63 down, 64-255 up)
            public Node? Parent { get; set; }

            // node is unique (for purposes of A* search) if position and state are the same
            public int HashCode { get => Position.GetHashCode() ^ State.GetHashCode(); }
        }

        // for comparing F-values.  if F is equal, compare hashcodes to ensure uniqueness.
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

        /*
            For part 2, we need to keep track of the node state (last N steps in a particular direction) in addition to the position.
            Note that the end state is not just reaching the end node, but also the correct state of the end node (>=4 steps in the current direction).
            https://stackoverflow.com/questions/39305377/dealing-with-a-time-based-constraint-in-a-star-on-2d-grid-world
            ^ Instead of a time constraint, we have a state-of-last-N-nodes constraint.  We need to treat this state as a third dimension to our search grid.
        */
        public List<Node> Search(int minDirSteps, int maxDirSteps)
        {
            var divisor = maxDirSteps+1;

            var startNode = new Node() { Position = (0,0) };
            var endNodes = new HashSet<(int, int)>() { (Grid[0].Length-1, Grid.Length-1) };

            // maintain the open list both as a hash for O(1) lookup by position+state, and as a sorted set to quickly get the node with the lowest F score
            var open = new Dictionary<int, Node>();
            var openF = new SortedSet<Node>(new NodeComparer());

            // for closed, we don't need the node itself, just its hash to keep track of them.
            var closed = new HashSet<int>();

            var path = new List<Node>();              // to quiet null warning; empty path will be returned if no path found

            open.Add(startNode.HashCode, startNode);
            openF.Add(startNode);

            var dirs = new List<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };

            /*var visited = 0;
            var sw = new Stopwatch();
            sw.Start();*/

            while (open.Any())
            {
                // get node with lowest F score
                var currentNode = openF.First();
                var currentHash = currentNode.HashCode;

                // remove from open list, add to closed list
                open.Remove(currentHash);
                openF.Remove(currentNode);
                closed.Add(currentHash);

                // if we've reached the end, we're done
                if (endNodes.Contains(currentNode.Position) && currentNode.State % divisor >= minDirSteps)
                {
                    var node = currentNode;
                    while (node != null)
                    {
                        path.Add(node);
                        node = node.Parent;
                    }

                    path.Reverse();
                    /*System.Console.WriteLine("found a path!");
                    foreach (var n in path)
                    {
                        System.Console.WriteLine("  " + n.Position + " " + n.G);
                    }*/

                    return path;
                }

                // get all adjacent nodes
                var adjacentNodes = new List<Node>();

                var x = currentNode.Position.Item1;
                var y = currentNode.Position.Item2;

                // find neighbors
                // for part 2, once we start going in a particular direction, we must continue in that direction for at least 4 and no more than 10 steps.
                // we can determine if a turn is allowed without considering each of the 4 directions.
                // if a turn is not allowed, then we don't need to loop over all 4 directions.
                // conveniently, the saved state helps a lot here.
                int? onlyDirAllowed = currentNode.Position == startNode.Position || currentNode.State % divisor >= minDirSteps ? null : currentNode.State / divisor;

                for (var dirIndex=0; dirIndex<4; dirIndex++)
                {
                    if (onlyDirAllowed.HasValue && dirIndex != onlyDirAllowed) continue;
                    var dir = dirs[dirIndex];

                    var newX = x + dir.Item1;
                    var newY = y + dir.Item2;

                    if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;           // out of bounds
                    if (newY == startNode.Position.Item2 && newX == startNode.Position.Item1) continue;            // don't revisit start
                    if (newX == currentNode.Parent?.Position.Item1 && newY == currentNode.Parent?.Position.Item2) continue;    // don't turn around

                    // for part 2, once we start going in a particular direction, we must continue in that direction for at least 4 and no more than 10 steps.
                    // to check for "at least 4", we need to check the last 3 steps (up to currentNode, ) and make sure they are all in the same direction.

                    // check straight-line constraint
                    var stepsInThisDir = currentNode.State / divisor == dirIndex ? currentNode.State % divisor : 0;
                    if (stepsInThisDir >= maxDirSteps) continue;

                    // allowable neighbor
                    var newNode = new Node() { Position = (newX, newY), Parent = currentNode, State = divisor*dirIndex+stepsInThisDir+1};
                    adjacentNodes.Add(newNode);
                }

                // loop through adjacent nodes
                foreach (var adjacentNode in adjacentNodes)
                {
                    var hash = adjacentNode.HashCode;
                    if (closed.Contains(hash)) continue;

                    adjacentNode.G = currentNode!.G + Grid[adjacentNode.Position.Item2][adjacentNode.Position.Item1];
                    // H (heuristic cost) is manhattan distance to end.  if multiple ends, use the closest one.
                    adjacentNode.H = endNodes.Min(p => Math.Abs(adjacentNode.Position.Item1 - p.Item1) + Math.Abs(adjacentNode.Position.Item2 - p.Item2));
                    adjacentNode.F = adjacentNode.G + adjacentNode.H;

                    if (open.ContainsKey(hash) && open[hash].G <= adjacentNode.G) continue;         // already in open list with lower G score

                    open.Add(hash, adjacentNode);
                    openF.Add(adjacentNode);
                }

                /*
                // benchmark
                visited++;
                if (visited % 10000 == 0)
                {
                    System.Console.WriteLine("visited " + visited + " nodes, open list size " + open.Count + ", current node " + currentNode.Position + " state " + (currentNode.State/11) + " in " + sw.ElapsedMilliseconds + "ms");
1                    sw.Restart();
                }
                */
            }

            Console.WriteLine("did not find a path!");
            return path;
        }
    }

    protected override long Part1()
    {
        return Input.Search(0, 3).Last().G;
    }

    protected override long Part2()
    {
        return Input.Search(4, 10).Last().G;
    }

    protected override Map Parse(string input)
    {
        var grid = input.Split('\n').Select(p => p.Trim()).Where(p => p != "").Select(p => p.Select(q => int.Parse(q.ToString())).ToArray()).ToArray();
        return new Map() { Grid = grid };
    }
}