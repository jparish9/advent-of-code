using AOC.Utils;

namespace AOC.AOC2024;

public class Day23 : Day<Day23.NetworkMap>
{
    protected override string? SampleRawInput { get => "kh-tc\nqp-kh\nde-cg\nka-co\nyn-aq\nqp-ub\ncg-tb\nvc-aq\ntb-ka\nwh-tc\nyn-cg\nkh-ub\nta-co\nde-co\ntc-td\ntb-wq\nwh-td\nta-ka\ntd-qp\naq-cg\nwq-ub\nub-vc\nde-ta\nwq-aq\nwq-vc\nwh-yn\nka-de\nkh-ta\nco-tc\nwh-qp\ntb-vc\ntd-yn"; }

    public class NetworkMap
    {
        public required List<NamedNode> Nodes;

        public required List<(string From, string To)> Edges;           // for part 2, original set of edges (not duplicated because of bidirectionality)

        public List<string> ConnectedSet = [];                          // the biggest connected set (cache for part 2)
    }

    protected override Answer Part1()
    {
        var cycles = new List<List<string>>();

        // start at each node and find all cycles of 3 nodes.  this could be made into a recursive function to support looking for arbitrary cycle length.
        foreach (var node in Input.Nodes)
        {
            var visited = new HashSet<NamedNode>() { node };
            foreach (var edge in node.Edges)
            {
                var next = edge.To;
                if (visited.Contains(next)) continue;
                visited.Add(next);
                foreach (var edge2 in next.Edges)
                {
                    var next2 = edge2.To;
                    if (visited.Contains(next2)) continue;
                    visited.Add(next2);
                    foreach (var edge3 in next2.Edges)
                    {
                        if (edge3.To == node)
                        {
                            cycles.Add([node.Name, next.Name, next2.Name]);
                        }
                    }
                    visited.Remove(next2);
                }
                visited.Remove(next);
            }
        }

        var distinct = cycles.Select(p => p.OrderBy(q => q).ToList()).Select(p => string.Join(",", p)).Distinct().Select(p => p.Split(",")).ToList();

        var ct = 0;
        foreach (var cycle in distinct)
        {
            if (cycle.Any(p => p.StartsWith('t'))) ct++;
        }

        return ct;
    }

    protected override Answer Part2()
    {
        // for every connection (2 nodes):
        //   initialize the connected set with the 2 nodes.
        //   for every node not part of the connected set:
        //     check if it is connected to every node in the current connected set.  add it to the connected set if it is.
        // this gets the right answer.  there may be a more efficient way to do this (this seems at least O(n^2), maybe O(n^3)), but it's fast enough for this input size (a few seconds).

        // this is apparently the "clique problem", and is NP-hard.
        // rewritten to use the Bron-Kerbosch algorithm, which is exactly what is needed here (a recursive backtracking algorithm to find all maximal cliques in an undirected graph).
        // the basic version 70% faster (less than a second).  the pivot version is even faster (~300ms).

        BronKerbosch([], Input.Nodes, []);
        return string.Join(",", Input.ConnectedSet);

        /*
        // original code:
        var largestConnectedSet = new HashSet<string>();

        foreach (var (From, To) in Input.Edges)
        {
            var connectedSet = new HashSet<string>() { From, To };

            foreach (var node in Input.Nodes)
            {
                if (connectedSet.Contains(node.Name)) continue;
                if (node.Edges.Select(p => p.To.Name).Intersect(connectedSet).Count() == connectedSet.Count)
                {
                    connectedSet.Add(node.Name);
                }
            }

            if (connectedSet.Count > largestConnectedSet.Count)
            {
                largestConnectedSet = connectedSet;
            }
        }
        return string.Join(",", largestConnectedSet.OrderBy(p => p));*/
    }

    // thanks, https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm !
    /*
        algorithm BronKerbosch2(R, P, X) is
        if P and X are both empty then
            report R as a maximal clique
        choose a pivot vertex u in P ⋃ X
        for each vertex v in P \ N(u) do
            BronKerbosch2(R ⋃ {v}, P ⋂ N(v), X ⋂ N(v))
            P := P \ {v}
            X := X ⋃ {v}
    */
    // could alter this a bit for reuse instead of having the side effect of setting Input.ConnectedSet (the maximal list of connected node names) and returning nothing.
    private void BronKerbosch(List<NamedNode> R, List<NamedNode> P, List<NamedNode> X)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            if (R.Count <= Input.ConnectedSet.Count) return;            // not maximal (so far)

            Input.ConnectedSet = [.. R.Select(p => p.Name).OrderBy(p => p)];
            return;
        }

        // choose a pivot node from P [union] X
        var pivot = P.Count > 0 ? P[0] : X[0];
        var pivotNeighbors = pivot.Edges.Select(p => p.To).ToList();

        var p = new List<NamedNode>(P);
        foreach (var node in P.Where(p => !pivotNeighbors.Contains(p)).ToList())            // don't recursively test neighbors of the pivot
        {
            var newR = new List<NamedNode>(R) { node };
            var newP = P.Where(p => node.Edges.Select(q => q.To).Contains(p)).ToList();
            var newX = X.Where(p => node.Edges.Select(q => q.To).Contains(p)).ToList();

            BronKerbosch(newR, newP, newX);

            P.Remove(node);
            X.Add(node);
        }
    }

    protected override NetworkMap Parse(string input)
    {
        var edges = input.Split("\n").Where(p => p != "").Select(p => p.Split("-"));
        var nodes = edges.SelectMany(p => p).Distinct().Select(p => new NamedNode() { Name = p }).ToList();

        foreach (var edge in edges)
        {
            var from = nodes.First(p => p.Name == edge[0]);
            var to = nodes.First(p => p.Name == edge[1]);
            // edges are bidirectional
            from.AddEdge(to);
            to.AddEdge(from);
        }

        return new NetworkMap() { Nodes = nodes, Edges = edges.Select(p => (p[0], p[1])).ToList() };
    }
}