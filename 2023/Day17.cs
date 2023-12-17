using System.Diagnostics;

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

            public bool Equals(Node node)
            {
                return Position == node.Position;
            }
        }

        /*
            I think we need to do something like this
            https://stackoverflow.com/questions/39305377/dealing-with-a-time-based-constraint-in-a-star-on-2d-grid-world
            but instead of a time constraint, we have a straight-line constraint,
            so we need to keep track of the last 3 steps taken and make sure we don't go more than 3 steps in any direction.
            nodes with the same position can be revisited if the last 3 steps are different.
        */

        public List<Node> Search()
        {
            var startNode = new Node() { Position = (0,0) };
            var endNodes = new HashSet<(int, int)>() { (Grid[0].Length-1, Grid.Length-1) };

            var open = new Dictionary<(int, int, int), Node>();  // for open we need to keep track of the positions and the order
            var closed = new HashSet<(int, int, int)>();         // for closed we only need to keep track of the positions (and check existence, not order)

            var path = new List<Node>();              // to quiet null warning; empty path will be returned if no path found

            open.Add((startNode.Position.Item1, startNode.Position.Item2, startNode.State), startNode);

            var visited = 0;
            var sw = new Stopwatch();
            sw.Start();

            while (open.Any())
            {
                // get node with lowest F score
                var currentNode = open.Values.OrderBy(p => p.F).First();

                // remove from open list, add to closed list
                open.Remove((currentNode.Position.Item1, currentNode.Position.Item2, currentNode.State));
                closed.Add((currentNode.Position.Item1, currentNode.Position.Item2, currentNode.State));

                // if we've reached the end, we're done
                if (endNodes.Contains(currentNode.Position) && currentNode.State % 11 >= 4)
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
                var dirs = new List<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };

                var x = currentNode.Position.Item1;
                var y = currentNode.Position.Item2;

                /*System.Console.Write($"Finding neighbors of {x},{y}, state " + (currentNode.State/11));
                var test = currentNode.Parent;
                for (var i=1; i<=5 && test != null; i++)
                {
                    System.Console.Write($" -> {test.Position.Item1},{test.Position.Item2}");
                    test = test.Parent;
                }
                System.Console.WriteLine();*/

                // find neighbors
                // for part 2, once we start going in a particular direction, we must continue in that direction for at least 4 and no more than 10 steps.
                // we can determine if a turn is allowed without considering each of the 4 directions.
                // if a turn is not allowed, then we don't need to loop over all 4 directions.
                // conveniently, the saved state helps a lot here.
                int? onlyDirAllowed = currentNode.Equals(startNode) || currentNode.State % 11 >= 4 ? null : currentNode.State / 11;

                for (var dirIndex=0; dirIndex<4; dirIndex++)
                {
                    if (onlyDirAllowed.HasValue && dirIndex != onlyDirAllowed) continue;
                    var dir = dirs[dirIndex];

                    var newX = x + dir.Item1;
                    var newY = y + dir.Item2;

                    if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;           // out of bounds
                    if (newY == startNode.Position.Item2 && newX == startNode.Position.Item1) continue;            // don't revisit start
                    if (newX == currentNode.Parent?.Position.Item1 && newY == currentNode.Parent?.Position.Item2) continue;    // don't turn around

                    var stepsInThisDir = 0;

                    // for part 2, once we start going in a particular direction, we must continue in that direction for at least 4 and no more than 10 steps.
                    // to check for "at least 4", we need to check the last 3 steps (up to currentNode, ) and make sure they are all in the same direction.

                    // check straight-line constraint
                    var checkNode = currentNode;
                    while (checkNode != null && stepsInThisDir <= 10)
                    {
                        if (checkNode.Position.Item1 != newX-dir.Item1*(stepsInThisDir+1) || checkNode.Position.Item2 != newY-dir.Item2*(stepsInThisDir+1))
                        {
                            break;
                        }

                        stepsInThisDir++;
                        checkNode = checkNode.Parent;
                    }

                    if (stepsInThisDir > 10) continue;

                    // allowable neighbor
                    //System.Console.WriteLine("  adding neighbor " + newX + "," + newY + " state " + (11*dirIndex + stepsInThisDir));
                    var newNode = new Node() { Position = (newX, newY), Parent = currentNode, State = currentNode.State = 11*dirIndex + stepsInThisDir};
                    adjacentNodes.Add(newNode);
                }

                // loop through adjacent nodes
                foreach (var adjacentNode in adjacentNodes)
                {
                    if (closed.Contains((adjacentNode.Position.Item1, adjacentNode.Position.Item2, adjacentNode.State))) continue;

                    adjacentNode.G = currentNode!.G + Grid[adjacentNode.Position.Item2][adjacentNode.Position.Item1];
                    // H (heuristic cost) is manhattan distance to end.  if multiple ends, use the closest one.
                    adjacentNode.H = endNodes.Min(p => Math.Abs(adjacentNode.Position.Item1 - p.Item1) + Math.Abs(adjacentNode.Position.Item2 - p.Item2));
                    adjacentNode.F = adjacentNode.G + adjacentNode.H;

                    if (open.ContainsKey((adjacentNode.Position.Item1, adjacentNode.Position.Item2, adjacentNode.State))
                        && open[(adjacentNode.Position.Item1, adjacentNode.Position.Item2, adjacentNode.State)].G <= adjacentNode.G) continue;

                    open.Add((adjacentNode.Position.Item1, adjacentNode.Position.Item2, adjacentNode.State), adjacentNode);
                }

                visited++;
                if (visited % 10000 == 0)
                {
                    System.Console.WriteLine("visited " + visited + " nodes, open list size " + open.Count + ", current node " + currentNode.Position + " state " + (currentNode.State/11) + " in " + sw.ElapsedMilliseconds + "ms");
                    sw.Restart();
                }
            }

            Console.WriteLine("did not find a path!");
            return path;
        }
    }

        // node class for A* search
    

    protected override long Part1()
    {
        return Input.Search().Last().G;
    }

    protected override long Part2()
    {
        throw new NotImplementedException();
    }

    protected override Map Parse(string input)
    {
        var grid = input.Split('\n').Select(p => p.Trim()).Where(p => p != "").Select(p => p.Select(q => int.Parse(q.ToString())).ToArray()).ToArray();
        return new Map() { Grid = grid };
    }
}