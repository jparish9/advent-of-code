using AOC.Utils;

namespace AOC.AOC2022;

public class Day16 : Day<Day16.Network>
{
    protected override string? SampleRawInput { get => "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\nValve BB has flow rate=13; tunnels lead to valves CC, AA\nValve CC has flow rate=2; tunnels lead to valves DD, BB\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\nValve EE has flow rate=3; tunnels lead to valves FF, DD\nValve FF has flow rate=0; tunnels lead to valves EE, GG\nValve GG has flow rate=0; tunnels lead to valves FF, HH\nValve HH has flow rate=22; tunnel leads to valve GG\nValve II has flow rate=0; tunnels lead to valves AA, JJ\nValve JJ has flow rate=21; tunnel leads to valve II"; }

    public class Network
    {
        public required List<Node> Nodes { get; set; }
    }

    public class Node : BaseNode<Node>
    {
        public required string Name { get; set; }
        public int FlowRate { get; set; }
    }

    protected override Answer Part1()
    {
        // this is a specialized graph search, where we have the option of remaining at a valve and opening it instead of moving to an adjacent valve.
        // there is also a step limit (30), where each move or open action is 1 step.
        // the goal is to maximize total flow released, which is the sum of (flow rate * remaining steps) for each valve opened.

        return Search(Input.Nodes.First());
    }

    public int Search(Node start)
    {
        var best = int.MinValue;

        var path = new List<Node>();
        var visited = new HashSet<Node>();
        var opened = new HashSet<Node>();

        Search(start, visited, path, 0, ref best, Graph<Node>.Maximize, opened, 0);

        return best;
    }

    private void Search(Node current, HashSet<Node> visited, List<Node> path, int weight, ref int best,
        Func<int, int, bool> compare, 
        HashSet<Node> opened, int steps)
    {
        //System.Console.WriteLine($"At node {current.Name}, weight {weight}, opened: {string.Join(",", opened.Select(n => n.Name))}, steps: {steps}");
        if (steps == 30 || opened.Count == Input.Nodes.Count(n => n.FlowRate > 0))
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

        if (!opened.Contains(current) && current.FlowRate > 0)
        {
            // open this valve instead of moving to a neighbor.  count the entire remaining flow from opening this valve.
            opened.Add(current);
            Search(current, visited, path, weight + current.FlowRate * (30 - steps - 1), ref best, compare, opened, steps + 1);
            opened.Remove(current);
        }

        // move to each neighbor.  unrestricted within the time limit.
        foreach (var edge in current.Edges)
        {
            Search(edge.To, visited, path, weight, ref best, compare, opened, steps + 1);
        }

        visited.Remove(current);
        path.Remove(current);
    }

    protected override Answer Part2()
    {
        throw new NotImplementedException();
    }

    protected override Network Parse(RawInput input)
    {
        var nodes = new Dictionary<string, Node>();
        // scan for all valve names first so we can allocate nodes
        foreach (var line in input.Lines())
        {
            var name = line.Substring(6, 2);         // "Valve AA ..."
            nodes[name] = new Node
            {
                Name = name
            };
        }

        foreach (var line in input.Lines())
        {
            // Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            var parts = line.Split([" has flow rate=", "; tunnels lead to valves ", "; tunnel leads to valve "], StringSplitOptions.RemoveEmptyEntries);
            var name = parts[0][6..];         // skip "Valve "
            var flowRate = int.Parse(parts[1]);
            var connections = parts[2].Split(", ").ToList();

            var node = nodes[name];

            node.FlowRate = flowRate;
            node.Edges = [.. connections.Select(c => new Edge<Node>() { From = node, To = nodes[c] })];
        }

        return new Network
        {
            Nodes = [.. nodes.ToList().Select(kv => kv.Value)]
        };
    }
}