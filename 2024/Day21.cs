using System.Text.RegularExpressions;

namespace AOC.AOC2024;

public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>
        (this IEnumerable<IEnumerable<T>> sequences) 
    { 
        IEnumerable<IEnumerable<T>> emptyProduct = [[]]; 
        return sequences.Aggregate( 
            emptyProduct, 
            (accumulator, sequence) => 
            from accseq in accumulator 
            from item in sequence 
            select accseq.Concat([item]));
    }
}

public class Day21 : Day<Day21.DoorCodes>
{
    protected override string? SampleRawInput { get => "029A\n980A\n179A\n456A\n379A"; }
    private static readonly (int X, int Y)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public class DoorCodes
    {
        public required List<string> Codes { get; set; }
    }

    public abstract class Keypad
    {
        public char Position;
        public Dictionary<char, ButtonNode> ButtonSet = [];

        private static Dictionary<(char, char), List<List<Move>>> _pathCache = new();
 
        protected void Initialize()
        {
            foreach (var button in ButtonSet)
            {
                // determine neighbors (manhattan distance of 1)
                var (x, y) = button.Value.Position;
                foreach (var (X, Y) in Directions)
                {
                    var b = ButtonSet.FirstOrDefault(p => p.Value.Position == (x + X, y + Y));
                    if (b.Value != null)
                    {
                        button.Value.Neighbors.Add(b.Value);
                    }
                }
            }
        }

        public List<string> DirectionalKeypadPresses(string code)
        {
            var allMoveSets = new List<List<List<Move>>>();
            
            foreach (var ch in code)
            {
                allMoveSets.Add(Press(ch));
            }

            // convert these to their strings first
            var allMoves = new List<List<string>>();
            foreach (var moveSet in allMoveSets)
            {
                var moves = new List<string>();
                foreach (var move in moveSet)
                {
                    moves.Add(string.Join("", move.Select(p => DirectionalKeypad.DirectionMap[p.Direction])));
                }
                allMoves.Add(moves);
            }

            // now get their cartesian product, and join into single strings
            return allMoves.CartesianProduct().Select(p => string.Join("", p)).ToList();
        }

        public List<List<Move>> Press(char dest)
        {
            // we need to enumerate ALL possible moves to dest from current position (avoiding the gap), and return them.
            // with the buttons implemented as nodes, we can use the modified djikstra from day 18 to return all paths.
            // it might be a little overkill (this grid is small and explicit enumeration is possible), but it is unlikely to be slow and is more general.

            if (_pathCache.ContainsKey((Position, dest)))
            {
                var result = _pathCache[(Position, dest)];
                Position = dest;
                return result;
            }

            var start = ButtonSet[Position];
            var (endX, endY) = ButtonSet[dest].Position;

            var weights = ButtonSet.Values.Select(p => (Node: p, details: (Previous: (ButtonNode?)null, Weight: int.MaxValue))).ToDictionary(p => p.Node, p => p.details);
            var queue = new PriorityQueue<ButtonNode, int>();

            var predecessors = new Dictionary<ButtonNode, List<ButtonNode>>();

            weights[start] = (null, 0);

            queue.Enqueue(start, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Position.X == endX && current.Position.Y == endY)
                {
                    var allPaths = FindAllPaths(current, predecessors, start);

                    // convert these to moves
                    var allMoves = new List<List<Move>>();

                    foreach (var path in allPaths)
                    {
                        var moves = new List<Move>();
                        for (var i=0; i<path.Count-1; i++)
                        {
                            moves.Add(new Move() { Direction = (path[i+1].Position.X - path[i].Position.X, path[i+1].Position.Y - path[i].Position.Y) });
                        }
                        // add a "move" at the end of each move list representing the button press
                        moves.Add(new Move() { Direction = (0, 0) });
                        allMoves.Add(moves);
                    }

                    // update our position for the next move
                    _pathCache[(Position, dest)] = allMoves;

                    Position = dest;

                    return allMoves;
                }

                var currentWeight = weights[current].Weight;

                foreach (var edge in current.Neighbors)
                {
                    var weight = weights[edge].Weight;
                    var newWeight = currentWeight + 1;

                    if (newWeight < weight)
                    {
                        weights[edge] = (current, newWeight);
                        predecessors[edge] = [current];
                        queue.Remove(edge, out _, out _);           // .NET 9
                        queue.Enqueue(edge, newWeight);
                    }
                    else if (newWeight == weight)
                    {
                        predecessors[edge].Add(current);            // save equivalent path
                    }
                }
            }

            System.Console.WriteLine("Didn't find a path!");
            return [];
        }

        private static List<List<ButtonNode>> FindAllPaths(ButtonNode current, Dictionary<ButtonNode, List<ButtonNode>> predecessors, ButtonNode start)
        {
            if (current == start)
                return [[current]];

            var paths = new List<List<ButtonNode>>();

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

    public class ButtonNode
    {
        public (int X, int Y) Position;
        public char Value;
        public List<ButtonNode> Neighbors = [];
    }


    public class NumericKeypad : Keypad
    {
        public NumericKeypad()
        {
            ButtonSet = new Dictionary<char, ButtonNode>() {
                ['1'] = new ButtonNode() { Value = '1', Position = (0, 2) },
                ['2'] = new ButtonNode() { Value = '2', Position = (1, 2) },
                ['3'] = new ButtonNode() { Value = '3', Position = (2, 2) },
                ['4'] = new ButtonNode() { Value = '4', Position = (0, 1) },
                ['5'] = new ButtonNode() { Value = '5', Position = (1, 1) },
                ['6'] = new ButtonNode() { Value = '6', Position = (2, 1) },
                ['7'] = new ButtonNode() { Value = '7', Position = (0, 0) },
                ['8'] = new ButtonNode() { Value = '8', Position = (1, 0) },
                ['9'] = new ButtonNode() { Value = '9', Position = (2, 0) },
                ['0'] = new ButtonNode() { Value = '0', Position = (1, 3) },
                ['A'] = new ButtonNode() { Value = 'A', Position = (2, 3) },
            };

            Position = 'A';
            
            Initialize();
        }
    }

    public class DirectionalKeypad : Keypad
    {
        public static readonly Dictionary<(int, int), char> DirectionMap = new()
        {
            [(0, -1)] = '^',
            [(1, 0)] = '>',
            [(0, 1)] = 'v',
            [(-1, 0)] = '<',
            [(0, 0)] = 'A'
        };

        public DirectionalKeypad()
        {
            ButtonSet = new Dictionary<char, ButtonNode>() {
                ['^'] = new ButtonNode() { Value = '^', Position = (1, 0) },
                ['A'] = new ButtonNode() { Value = 'A', Position = (2, 0) },
                ['<'] = new ButtonNode() { Value = '<', Position = (0, 1) },
                ['v'] = new ButtonNode() { Value = 'v', Position = (1, 1) },
                ['>'] = new ButtonNode() { Value = '>', Position = (2, 1) }
            };

            Position = 'A';

            Initialize();
        }
    }

    public class Move
    {
        public (int X, int Y) Direction;            // 0,0 for button press
    }

    protected override Answer Part1()
    {
        var totalComplexity = 0;

        var keypad = new NumericKeypad();
        var dirKeypad1 = new DirectionalKeypad();
        var dirKeypad2 = new DirectionalKeypad();

        foreach (var code in Input.Codes)
        {
            var minLength = int.MaxValue;
            string aMove = "";

            var moves = keypad.DirectionalKeypadPresses(code);

            foreach (var move in moves)
            {
                var moves2 = dirKeypad1.DirectionalKeypadPresses(move);

                foreach (var move2 in moves2)
                {
                    var moves3 = dirKeypad2.DirectionalKeypadPresses(move2);
                    var len = moves3.Select(p => p.Length).Min();
                    if (len < minLength)
                    {
                        minLength = len;
                        aMove = moves3.First();
                    }
                }
            }

            var complexity = int.Parse(Regex.Replace(code, "[A-Z]", ""));
            totalComplexity += aMove.Length * complexity;
        }

        return totalComplexity;
    }

    protected override Answer Part2()
    {
        throw new NotImplementedException();
    }

    protected override DoorCodes Parse(string input)
    {
        return new DoorCodes() { Codes = input.Split("\n").Where(p => p != "").ToList() };
    }
}