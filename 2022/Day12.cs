namespace AOC.AOC2022;

public class Day12 : Day<Day12.Map>
{
    protected override string? SampleRawInput { get => "Sabqponm\nabcryxxl\naccszExk\nacctuvwj\nabdefghi"; }

    public class Map
    {
        public required char[][] Grid { get; set; }

        // "Start" is E because we are searching backwards
        public (int, int) Start()
        {
            var found = Find('E', 1);
            if (!found.Any()) throw new Exception("No start found");

            return found.First();
        }

        // find all possible ends
        public HashSet<(int, int)> End(char endLevel)
        {
            return Find(endLevel);
        }

        private HashSet<(int, int)> Find(char level, int? max = null)
        {
            var found = new HashSet<(int, int)>();

            for (var y = 0; y < Grid.Length; y++)
            {
                for (var x = 0; x < Grid[0].Length; x++)
                {
                    if (Grid[y][x] == level)
                    {
                        found.Add((x, y));
                        if (max != null && found.Count >= max.Value) return found;
                    }
                }
            }

            return found;
        }

        // node class for A* search
        private class Node
        {
            public int F { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public (int, int) Position { get; set; }
            public Node? Parent { get; set; }

            public bool Equals(Node node)
            {
                return Position == node.Position;
            }
        }

        // A* search from 'E' to any with given endLevel, return shortest path
        public List<(int, int)> Search(char endLevel)
        {
            var startNode = new Node() { Position = Start() };
            var endNodes = End(endLevel);

            var endLevelInt = endLevel == 'S' ? 'a'-1 : endLevel;

            var open = new List<Node>();
            var closed = new HashSet<(int, int)>();         // for closed we only need to keep track of the positions (and check existence, not order)

            var path = new List<(int, int)>();              // to quiet null warning; empty path will be returned if no path found

            open.Add(startNode);

            while (open.Any())
            {
                // get node with lowest F score
                var currentNode = open.OrderBy(p => p.F).First();

                // remove from open list, add to closed list
                open.Remove(currentNode);
                closed.Add(currentNode.Position);

                // if we've reached the end, we're done
                if (endNodes.Contains(currentNode.Position))
                {
                    var node = currentNode;
                    while (node != null)
                    {
                        path.Add(node.Position);
                        node = node.Parent;
                    }

                    path.Reverse();
                    return path;
                }

                // get all adjacent nodes
                var adjacentNodes = new List<Node>();
                var dirs = new List<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };

                var x = currentNode.Position.Item1;
                var y = currentNode.Position.Item2;

                foreach (var dir in dirs)
                {
                    var newX = x + dir.Item1;
                    var newY = y + dir.Item2;

                    if (newX < 0 || newY < 0 || newY >= Grid.Length || newX >= Grid[0].Length) continue;           // out of bounds
                    if (newY == startNode.Position.Item2 && newX == startNode.Position.Item1) continue;                        // don't revisit start

                    // if searching backwards, we can go down at most 1, but up any amount
                    var currentLevel = Grid[y][x] == 'E' ? 'z'+1 : Grid[y][x];
                    var neighborLevel = Grid[newY][newX] == endLevel ? endLevelInt : Grid[newY][newX];
                    
                    if (neighborLevel < currentLevel-1) continue;

                    // allowable neighbor
                    var newNode = new Node() { Position = (newX, newY), Parent = currentNode };
                    adjacentNodes.Add(newNode);
                }

                // loop through adjacent nodes
                foreach (var adjacentNode in adjacentNodes)
                {
                    // if node is in closed list, skip it
                    if (closed.Contains(adjacentNode.Position)) continue;

                    // G (actual cost) is just number of steps (vertical/horizontal) taken so far
                    adjacentNode.G = currentNode.G + 1;

                    var inOpen = open.FirstOrDefault(p => p.Equals(adjacentNode));
                    if (inOpen != null && adjacentNode.G >= inOpen.G) continue;     // if node is already in open list, skip it (this path is not better)

                    // H (heuristic cost) is manhattan distance to end.  if multiple ends, use the closest one.
                    adjacentNode.H = endNodes.Min(p => Math.Abs(adjacentNode.Position.Item1 - p.Item1) + Math.Abs(adjacentNode.Position.Item2 - p.Item2));
                    adjacentNode.F = adjacentNode.G + adjacentNode.H;

                    open.Add(adjacentNode);
                }
            }

            Console.WriteLine("did not find a path!");
            return path;
        }
    }

    protected override long Part1()
    {
        return Input.Search('S').Count-1;
    }

    protected override long Part2()
    {
        return Input.Search('a').Count-1;
    }

    protected override Map Parse(string input)
    {
        var grid = input.Split('\n').Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
        return new Map() { Grid = grid };
    }
}