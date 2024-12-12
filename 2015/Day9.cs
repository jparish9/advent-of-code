using AOC.Utils;

namespace AOC.AOC2015;

public class Day9 : Day<Graph>
{
    protected override string? SampleRawInput { get => "London to Dublin = 464\nLondon to Belfast = 518\nDublin to Belfast = 141"; }

    protected override Answer Part1()
    {
        return Input.Search(compare: Graph.Minimize);       // try all starts, traversing the whole graph, minimizing total Edge.Weight
    }

    protected override Answer Part2()
    {
        return Input.Search(compare: Graph.Maximize);
    }

    protected override Graph Parse(string input)
    {
        var graph = new Graph();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(" ");
            var from = parts[0];
            var to = parts[2];
            var distance = int.Parse(parts[4]);

            var fromNode = graph.Nodes.FirstOrDefault(p => p.Name == from);
            if (fromNode == null)
            {
                fromNode = new Node() { Name = from };
                graph.Nodes.Add(fromNode);
            }

            var toNode = graph.Nodes.FirstOrDefault(p => p.Name == to);
            if (toNode == null)
            {
                toNode = new Node() { Name = to };
                graph.Nodes.Add(toNode);
            }

            fromNode.Edges.Add(new Edge() { From = fromNode, To = toNode, Weight = distance });
            toNode.Edges.Add(new Edge() { From = toNode, To = fromNode, Weight = distance });
        }

        return graph;
    }
}