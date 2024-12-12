using AOC.Utils;

namespace AOC.AOC2023;

public class Day8 : Day<Day8.RouteDef>
{
    //public override string? SampleInput { get => "RL\n\nAAA = (BBB, CCC)\nBBB = (DDD, EEE)\nCCC = (ZZZ, GGG)\nDDD = (DDD, DDD)\nEEE = (EEE, EEE)\nGGG = (GGG, GGG)\nZZZ = (ZZZ, ZZZ)"; }
    protected override string? SampleRawInput { get => "LLR\n\nAAA = (BBB, BBB)\nBBB = (AAA, ZZZ)\nZZZ = (ZZZ, ZZZ)"; }
    protected override string? SampleRawInputPart2 { get => "LR\n\n11A = (11B, XXX)\n11B = (XXX, 11Z)\n11Z = (11B, XXX)\n22A = (22B, XXX)\n22B = (22C, 22C)\n22C = (22Z, 22Z)\n22Z = (22B, 22B)\nXXX = (XXX, XXX)"; }

    public class RouteDef
    {
        public required char[] Path { get; set; }

        public Dictionary<string, Node> Nodes { get; set; } = new();
    }

    public class Node
    {
        public required string Name { get; set; }
        public required string Left { get; set; }
        public required string Right { get; set; }
        public required bool EndsInZ { get; set; }          // do this up-front
    }

    protected override Answer Part1()
    {
        var path = Input.Path;
        var node = Input.Nodes["AAA"];

        var i=0;
        var pathLength = 0;
        while (node.Name != "ZZZ")
        {
            var pathChar = path[i];
            node = Input.Nodes[pathChar == 'L' ? node.Left : node.Right];
            i++;
            pathLength++;
            if (i == path.Length) i = 0;
        }

        return pathLength;
    }

    protected override Answer Part2()
    {
        // okay, well this was fun.  brute-force got to 150M in 7 minutes before I gave up.  Reddit says the iterative step count is like 100T.
        // is there a better way?
        // as it turns out, there is.  we can run each node ending in A until it stops, instead of running them all at once, and keep track of all of the path lengths.
        // for this to work, the input has to cycle once we get to Z.  it does, but that means this doesn't really solve the general problem for all inputs.
        // it could easily be the case that after one path gets to Z it never hits Z again.
        // but all of these inputs cycle, and conveniently [enter loop length] == [cycle length] for every starting A node.
        // so we can just take the LCM of the path lengths (this will be the point where they ALL hit a cycle end at the same time) and be done with it.
        // there is an algorithm to handle the more general case where [enter loop length] doesn't necessarily equal [cycle length] (but everything still cycles):
        // https://math.stackexchange.com/questions/2218763/how-to-find-lcm-of-two-numbers-when-one-starts-with-an-offset/3864593#3864593

        var path = Input.Path;
        var pathLengths = new List<long>();
        foreach (var node in Input.Nodes.Values.Where(p => p.Name.EndsWith("A")))
        {
            var currentNode = node;
            var i=0;
            var pathLength = 0;
            while (!currentNode.EndsInZ)
            {
                var pathChar = path[i];
                currentNode = Input.Nodes[pathChar == 'L' ? currentNode.Left : currentNode.Right];
                i++;
                pathLength++;
                if (i == path.Length) i = 0;
            }
            pathLengths.Add(pathLength);
        }

        return pathLengths.Aggregate(Maths.LeastCommonMultiple);
        
        /*
        BRUTE-FORCE (150M in 7 minutes, actual solution is in the trillions, good luck)
        var startingNodes = Input.Nodes.Where(p => p.Key.EndsWith("A")).Select(p => p.Value).ToList();

        // make a copy of these, so we can modify them in the loop instead of instantiating new ones each time
        var nodes = startingNodes.Select(p => new Node() { Name = p.Name, Left = p.Left, Right = p.Right, EndsInZ = p.EndsInZ }).ToList();
        
        var i=0;
        var pathLength = 0;
        var sw = new Stopwatch();
        sw.Start();
        while (nodes.Any(p => !p.EndsInZ))
        {
            var pathChar = path[i];
            foreach (var node in nodes)
            {
                var nextNode = pathChar == 'L' ? Input.Nodes[node.Left] : Input.Nodes[node.Right];
                node.Left = nextNode.Left;
                node.Right = nextNode.Right;
                node.EndsInZ = nextNode.EndsInZ;
            }
            i++;
            pathLength++;
            if (i == path.Length) i = 0;
            if (pathLength % 10000000 == 0)
            {
                Console.WriteLine($"{pathLength} {sw.ElapsedMilliseconds}");
            }
        }

        return pathLength;*/
    }


    protected override RouteDef Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();

        var nodes = new Dictionary<string, Node>();

        for (var i=1; i<lines.Length; i++)
        {
            var line = lines[i];

            var parts = line.Split(" = ");

            var name = parts[0];
            var left = parts[1].Split(", ")[0].Trim('(', ')');
            var right = parts[1].Split(", ")[1].Trim('(', ')');

            nodes.Add(name, new Node() { Name = name, Left = left, Right = right, EndsInZ = name.EndsWith("Z") });
        }
        
        return new RouteDef() { Path = lines[0].ToCharArray(), Nodes = nodes };
    }
}