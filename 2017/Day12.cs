namespace AOC.AOC2017;

public class Day12 : Day<Day12.Network>
{
    protected override string? SampleRawInput { get => "0 <-> 2\n1 <-> 1\n2 <-> 0, 3, 4\n3 <-> 2, 4\n4 <-> 2, 3, 6\n5 <-> 6\n6 <-> 4, 5"; }

    public class Network
    {
        public required Dictionary<int, List<int>> Connections { get; set; }
    }

    protected override Answer Part1()
    {
        var group = GetGroup(0);
        return group.Count;
    }

    protected override Answer Part2()
    {
        var groups = 0;
        while (Input.Connections.Count > 0)
        {
            var startNode = Input.Connections.Keys.First();
            var group = GetGroup(startNode);

            // remove all nodes in this group from the connections
            foreach (var node in group)
            {
                Input.Connections.Remove(node);
            }

            groups++;
        }

        return groups;
    }

    private HashSet<int> GetGroup(int startNode)
    {
        var visited = new HashSet<int>();
        var toVisit = new Queue<int>();

        toVisit.Enqueue(startNode);         // find all visitable from start node

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            if (visited.Contains(current)) continue;         // already visited

            visited.Add(current);

            foreach (var neighbor in Input.Connections[current])
            {
                if (!visited.Contains(neighbor))
                {
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        return visited;
    }

    protected override Network Parse(RawInput input)
    {
        var connections = new Dictionary<int, List<int>>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" <-> ");
            var node = int.Parse(parts[0]);
            var linkedNodes = parts[1].Split(", ").Select(int.Parse).ToList();

            connections[node] = linkedNodes;
        }

        return new Network
        {
            Connections = connections
        };
    }
}