using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Z3;

namespace AOC.AOC2025;

public partial class Day10 : Day<Day10.Factory>
{
    //protected override string? SampleRawInput { get => "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}"; }

    protected override string? SampleRawInput { get => "[#.#.#.#] (0,1,6) (2,4) (0,3,5,6) (0,3,4,6) (1,2,3) (3,4) {37,29,22,54,21,17,37}"; }

    public class Factory
    {
        public required List<Machine> Machines { get; set; }

        public Dictionary<(int machineId, string joltageKey), int> Cache { get; set; } = new();          // memoization for part 2
    }

    public class Machine
    {
        public required string LightDiagram { get; set; }
        public required List<List<int>> WiringSchematics { get; set; }
        public required List<int> JoltageRequirements { get; set; }
    }

    protected override Answer Part1()
    {
        var totalPresses = 0;
        foreach (var machine in Input.Machines)
        {
            // count the solution with the minimum number of total presses
            var sols = LightUp([.. machine.LightDiagram.Select(c => c == '#')], machine.WiringSchematics);
            foreach (var sol in sols)
            {
                System.Console.WriteLine("  Solution found: {" + string.Join(',', sol) + "} with total presses " + sol.Sum());
            }
            totalPresses += sols.Select(p => p.Sum()).Min();
        }

        return totalPresses;
    }

    // return ALL solutions that give the target state
    private static List<List<int>> LightUp(bool[] desiredState, List<List<int>> wiring)
    {
        var allSolutions = new List<List<int>>();
        // pressing any button more than once is pointless, so we just have to check every combination of pressing every button zero or once.
        for (var i=1; i < (1 << wiring.Count); i++)           // bitmask (e.g. for 5 buttons, 00001 to 11111)
        {
            // initialize presses to empty [0, 0..]
            var presses = new List<int>();
            for (var b=0; b < wiring.Count; b++)
            {
                presses.Add(0);
            }

            var state = new bool[desiredState.Length];           // initially all false (off)

            for (var b=0; b < wiring.Count; b++)
            {
                if ((i & (1 << b)) == 0) continue;          // this button not pressed
                presses[b]++;

                // toggle all lights this button controls
                foreach (var lightIndex in wiring[b])
                {
                    state[lightIndex] = !state[lightIndex];
                }
            }

            if (state.SequenceEqual(desiredState))
            {
                allSolutions.Add(presses);
            }
        }

        return allSolutions;
    }

    // this implementation of the clever solution from https://www.reddit.com/r/adventofcode/comments/1pk87hl/2025_day_10_part_2_bifurcate_your_way_to_victory/
    // is nearly working, but one of the input machines [#.#.#.#] (0,1,6) (2,4) (0,3,5,6) (0,3,4,6) (1,2,3) (3,4) {37,29,22,54,21,17,37}
    // runs into a state where it can't be reduced.  this is clearly incorrect because z3 finds a solution with 67 presses.
    protected Answer Part2Old2()
    {
        // we can use part 1 here!
        // consider the target joltages by their parity (even/odd).
        // we need to consider the minimium presses to achieve the required parity, even/odd, or off/on.  sound familiar?
        // once parity is achieved, we are left with remaining joltages that are all even.
        // halve them until at least one is odd and repeat.
        // once the joltages are all zero, we are done.
        var totalPresses = 0;
        for (var i=0; i < Input.Machines.Count; i++)
        {
            var result = CountToJoltage(Input.Machines[i].JoltageRequirements, Input.Machines[i].WiringSchematics, i);
            if (result == 100000000)
            {
                System.Console.WriteLine("No solution found for machine with wiring " + string.Join(" | ", Input.Machines[i].WiringSchematics.Select(w => "(" + string.Join(',', w) + ")")) + " and joltage requirements {" + string.Join(',', Input.Machines[i].JoltageRequirements) + "}");
            }
            totalPresses += result;
        }

        return totalPresses;
    }

    private int CountToJoltage(List<int> joltage, List<List<int>> wiring, int machineId, int depth = 0)
    {
        if (joltage.All(j => j == 0)) return 0;         // we are done

        var joltageKey = string.Join(',', joltage);

        if (Input.Cache.TryGetValue((machineId, joltageKey), out var cachedTotal)) return cachedTotal;         // check cache

        // if we are starting out with an inital state where the joltages are all even, halve them all until we find an odd one, and save the base multilpier.
        var baseMult = 1;
        var currentJoltage = new List<int>(joltage);
        while (currentJoltage.All(j => j % 2 == 0) && currentJoltage.Any(j => j > 0))
        {
            baseMult *= 2;
            for (var i=0; i < currentJoltage.Count; i++)
            {
                currentJoltage[i] /= 2;
            }
        }
        if (currentJoltage.All(j => j == 0)) return 0;         // we are done


        var total = 100000000;          // use a large flag value to return if no configurations are possible

        var allPatterns = LightUp(currentJoltage.Select(j => (j % 2) == 1).ToArray(), wiring);

        System.Console.WriteLine(new string(' ', depth * 2) + "Joltage: " + string.Join(',', currentJoltage) + ", found " + allPatterns.Count + " patterns");

        foreach (var pattern in allPatterns)
        {
            System.Console.WriteLine(new string(' ', depth * 2) + "  Considering pattern {" + string.Join(',', pattern) + "} with starting joltage " + string.Join(',', currentJoltage));

            // apply this pattern
            var newJoltage = new List<int>(currentJoltage);
            for (var i=0; i < pattern.Count; i++)
            {
                if (pattern[i] == 1)
                {
                    // toggle all lights this button controls
                    foreach (var lightIndex in wiring[i])
                    {
                        newJoltage[lightIndex]--;
                    }
                }
            }

            if (newJoltage.Any(j => j < 0)) continue;          // invalid pattern

            System.Console.WriteLine(new string(' ', depth * 2) + "  Applying valid pattern {" + string.Join(',', pattern) + "} gives new joltage " + string.Join(',', newJoltage));

            // halve all even joltages until we find an odd one or everything becomes zero
            var mult = 1;

            while (newJoltage.All(j => j % 2 == 0) && newJoltage.Any(j => j > 0))
            {
                mult *= 2;
                for (var i=0; i < newJoltage.Count; i++)
                {
                    newJoltage[i] /= 2;
                }
            }

            total = Math.Min(total, pattern.Sum() + baseMult * mult * CountToJoltage(newJoltage, wiring, machineId, depth+1));
        }

        Input.Cache[(machineId, joltageKey)] = total;         // store in cache
        return total;
    }

    // oh my god.
    // I spent so much time on this one, finally realizing that each joltage requiredment has to be met EXACTLY, not just minimally.
    // that makes the problem NOT a linear programming minimization problem!
    // (it's possible to get to >= all joltages with fewer total presses than with an exact solution,
    //   mostly by mashing the buttons that increase the most number of joltages.  :facepalm:)
    // instead it's an system of equations with more variables than equations (i.e. multiple solutions exist),
    // and we need to find the solution with the minimum sum of variables (total button presses).
    // I ended up trying out Z3 here, with credit to https://github.com/mohammedsouleymane/AdventOfCode/blob/main/AdventOfCode/Aoc2025/Day10.cs
    protected override Answer Part2()
    {
        // e.g. for the first sample input, with four joltages and six buttons given by (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
        // Minimize: x1 + x2 + x3 + x4 + x5 + x6
        // Subject to:
        // x5 + x6 = 3
        // x2 + x6 = 5
        // x3 + x4 + x5 = 4
        // x1 + x2 + x4 = 7
        // x1..6 >= 0
        // x1..6 all integers

        var totalMinPresses = 0;
        foreach (var machine in Input.Machines)
        {
            var numJoltages = machine.JoltageRequirements.Count;
            var numButtons = machine.WiringSchematics.Count;

            var ctx = new Context();

            var variables = machine.WiringSchematics.Select(p => ctx.MkIntConst(string.Join(",", p))).ToList();

            var opt = ctx.MkOptimize();
            for (var i=0; i<machine.JoltageRequirements.Count; i++)
            {
                List<IntExpr> vars = [];
                for (var j=0; j < machine.WiringSchematics.Count; j++)
                {
                    // does button j control joltage i?
                    if (machine.WiringSchematics[j].Contains(i))
                        vars.Add(variables[j]);
                }
                var sum = ctx.MkAdd(vars);
                var constraint = ctx.MkEq(sum, ctx.MkInt(machine.JoltageRequirements[i]));
                opt.Add(constraint);
            }

            var total = ctx.MkAdd(variables);
            opt.MkMinimize(total);
            foreach (var v in variables)
            {
                opt.Add(ctx.MkGe(v, ctx.MkInt(0)));         // all >= 0
            }

            var result = opt.Check();
            var presses = 0;
            if (result == Status.SATISFIABLE)
            {
                var model = opt.Model;
                foreach (var v in variables)
                {
                    var val = (model.Evaluate(v) as IntNum)!.Int;
                    presses += val;
                }

                totalMinPresses += presses;

                //System.Console.WriteLine("Min presses for machine with joltage {" + string.Join(',', machine.JoltageRequirements) + "} = " + presses + ", solution is {" + string.Join(',', variables.Select(v => (model.Evaluate(v) as IntNum)!.Int)) + "}");
            }
            else
            {
                System.Console.WriteLine("Z3 failed to solve machine with wiring " + string.Join(" | ", machine.WiringSchematics.Select(w => "(" + string.Join(',', w) + ")")) + " and joltage requirements {" + string.Join(',', machine.JoltageRequirements) + "}: " + result);
            }


            // verify the solution.
            var testJoltage = new List<int>();
            for (var j=0; j < numJoltages; j++)
            {
                testJoltage.Add(0);
            }

            for (var b=0; b < numButtons; b++)
            {
                var pressCount = (opt.Model.Evaluate(variables[b]) as IntNum)!.Int;
                if (pressCount > 0)
                {
                    foreach (var lightIdx in machine.WiringSchematics[b])
                    {
                        testJoltage[lightIdx] += pressCount;
                    }
                }
            }

            var isSus = false;
            for (var j=0; j < numJoltages; j++)
            {
                if (testJoltage[j] != machine.JoltageRequirements[j])
                {
                    isSus = true;
                    break;
                }
            }

            if (isSus)
            {
                System.Console.WriteLine("Machine with wiring " + string.Join(" | ", machine.WiringSchematics.Select(w => "(" + string.Join(',', w) + ")")) + " and joltage requirements {" + string.Join(',', machine.JoltageRequirements) + "}:");
                System.Console.WriteLine("  Suspect result, tested joltage " + string.Join(',', testJoltage) + " does not meet requirements " + string.Join(',', machine.JoltageRequirements));
            }
            /*else
            {
                System.Console.WriteLine("Verified, final joltage " + string.Join(',', testJoltage) + " meets requirements " + string.Join(',', machine.JoltageRequirements));
            }*/
        }

        return totalMinPresses;
    }

    protected override Factory Parse(RawInput input)
    {
        var machines = new List<Machine>();
        foreach (var line in input.Lines())
        {
            var diagram = line.Split('[')[1].Split(']')[0];

            // parse (N,N,N) groups
            var wiring = new List<List<int>>();
            var wiringParts = SchematicRegex().Matches(line);
            foreach (Match match in wiringParts)
            {
                var nums = match.Groups[1].Value.Split(',').Select(s => int.Parse(s.Trim())).ToList();
                wiring.Add(nums);
            }

            var joltage = line.Split("{")[1].Split("}")[0].Split(',').Select(s => int.Parse(s.Trim())).ToList();

            machines.Add(new Machine
            {
                LightDiagram = diagram,
                WiringSchematics = wiring,
                JoltageRequirements = joltage
            });
        }

        return new Factory() { Machines = machines };
    }

    [GeneratedRegex(@"\((.*?)\)")]
    private static partial Regex SchematicRegex();
}
