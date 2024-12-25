using System.Text.RegularExpressions;
using AOC.Utils;

namespace AOC.AOC2024;

public partial class Day21 : Day<Day21.DoorCodes>
{
    protected override string? SampleRawInput { get => "029A\n980A\n179A\n456A\n379A"; }

    public partial class DoorCodes
    {
        public required List<string> Codes { get; set; }

        public long GetComplexity(int robots)
        {
            // shoutout to this thread: https://www.reddit.com/r/adventofcode/comments/1hj8380/2024_day_21_part_2_i_need_help_three_days_in_row/
            // a few key observations:
            // - the cost of moving any directional keypad from X to Y doesn't depend on the directing keypad or any keypads that move _that_ directing keypad, since they all start and end at 'A' for any given move.
            // - thus the cost is only dependent on the current and target position on the current keypad, from which we can compute the (best) cost to direct that movement on the directing keypad.
            // - we can iterate up the chain for 2 (part 1), 25 (part 2), or any number of keypads with linear time and only a fixed memory cost.
            var dirKeypad = new DirectionalKeypad();

            var costs = new Dictionary<(char, char), long>();

            // base costs
            foreach (var button1 in dirKeypad.ButtonSet.Values)
            {
                foreach (var button2 in dirKeypad.ButtonSet.Values)
                {
                    dirKeypad.Position = button1.Value;
                    if (button1.Value == button2.Value) continue;
                    costs[(button1.Value, button2.Value)] = dirKeypad.FindPaths(button2.Value).Select(p => p.Count).Min();
                }
            }

            // now, for each pair of buttons on the next keypad, find the lowest cost to go from any button to any other button, using costs from the previous keypad.
            // replace costs and continue with the next keypad.
            for (var j=0; j<robots-1; j++)
            {
                var nextCosts = new Dictionary<(char, char), long>();
                
                foreach (var button1 in dirKeypad.ButtonSet.Values)
                {
                    foreach (var button2 in dirKeypad.ButtonSet.Values)
                    {
                        if (button1.Value == button2.Value) continue;

                        dirKeypad.Position = button1.Value;

                        var paths = dirKeypad.DirectionalKeypadPresses(button2.Value.ToString());
                        var minCost = long.MaxValue;

                        foreach (var path in paths)
                        {
                            var thisCost = costs[('A', path[0])];
                            for (var i=1; i<path.Length; i++)
                            {
                                thisCost += path[i-1] == path[i] ? 1 : costs[(path[i-1], path[i])];
                            }
                            if (thisCost < minCost) minCost = thisCost;
                        }

                        nextCosts[(button1.Value, button2.Value)] = minCost;
                    }
                }

                costs = nextCosts;
            }

            // now that we know the lowest cost for the highest-order robot arm to recursively do all of the moves, find all paths for the input and pick the shortest one.
            var numKeypad = new NumericKeypad();
            var complexity = 0L;
            foreach (var str in Codes)
            {
                var code = str.ToCharArray();
                var minCost = long.MaxValue;

                var paths = numKeypad.DirectionalKeypadPresses(str);

                foreach (var path in paths)
                {
                    var thisCost = costs[('A', path[0])];
                    for (var i=1; i<path.Length; i++)
                    {
                        thisCost += path[i-1] == path[i] ? 1 : costs[(path[i-1], path[i])];         // if repeating the same button, cost is 1 since we are back at 'A' and just need to press the button again.
                    }
                    if (thisCost < minCost) minCost = thisCost;
                }

                var numeric = int.Parse(MatchAlphabet().Replace(str, ""));
                complexity += minCost * numeric;
            }

            return complexity;
        }

        [GeneratedRegex("[A-Z]")]
        private static partial Regex MatchAlphabet();
    }

    public abstract class Keypad
    {
        public char Position;
        public Dictionary<char, ButtonNode> ButtonSet = [];

        private static Dictionary<(char, char), List<List<Move>>> _pathCache = [];
 
        protected void Initialize()
        {
            foreach (var button in ButtonSet)
            {
                // determine neighbors (manhattan distance of 1)
                var (x, y) = button.Value.Position;
                foreach (var (dirX, dirY) in GridCardinals)
                {
                    var b = ButtonSet.FirstOrDefault(p => p.Value.Position == (x + dirX, y + dirY));
                    if (b.Value != null)
                    {
                        button.Value.AddEdge(b.Value, 1);
                    }
                }
            }
        }

        public List<string> DirectionalKeypadPresses(string code)
        {
            var allMoveSets = new List<List<List<Move>>>();
            
            foreach (var ch in code)
            {
                allMoveSets.Add(FindPaths(ch));
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

        public List<List<Move>> FindPaths(char dest)
        {
            // we need to enumerate ALL possible moves to dest from current position (avoiding the gap), and return them.
            // with the buttons implemented as nodes, we can use the modified djikstra from day 18 to return all paths.
            // djikstra might be a little overkill (this grid is small and explicit enumeration is possible), but it is unlikely to be slow and is more general.

            if (_pathCache.ContainsKey((Position, dest)))
            {
                var result = _pathCache[(Position, dest)];
                Position = dest;
                return result;
            }

            var allPaths = Djikstra<ButtonNode>.Search(ButtonSet[Position], [.. ButtonSet.Values], p => p.Value == dest);

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

            // cache the found paths and update our position for the next move
            _pathCache[(Position, dest)] = allMoves;
            Position = dest;

            return allMoves;
        }
    }

    public class ButtonNode : DjikstraNode<ButtonNode>
    {
        public (int X, int Y) Position;
        public char Value;
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
        return Input.GetComplexity(2);
    }

    protected override Answer Part2()
    {
        return Input.GetComplexity(25);
    }

    protected override DoorCodes Parse(RawInput input)
    {
        return new DoorCodes() { Codes = input.Lines().ToList() };
    }
}