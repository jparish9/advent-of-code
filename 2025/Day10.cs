using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Z3;

namespace AOC.AOC2025;

public partial class Day10 : Day<Day10.Factory>
{
    protected override string? SampleRawInput { get => "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}"; }

    public class Factory
    {
        public required List<Machine> Machines { get; set; }
    }

    public class Machine
    {
        public required string LightDiagram { get; set; }
        public required List<List<int>> WiringSchematics { get; set; }
        public required List<int> JoltageRequirements { get; set; }
    }

    protected override Answer Part1()
    {
        var totalMinPresses = 0;
        foreach (var machine in Input.Machines)
        {
            var minPresses = int.MaxValue;

            // pressing any button more than once is pointless, so we just have to check every combination of pressing every button zero or once.
            var onState = machine.LightDiagram.Select(c => c == '#').ToArray();

            for (var i=1; i < (1 << machine.WiringSchematics.Count); i++)           // bitmask (e.g. for 5 buttons, 00001 to 11111)
            {
                var presses = 0;
                var state = new bool[onState.Length];           // initially all false (off)

                for (var b=0; b < machine.WiringSchematics.Count; b++)
                {
                    if ((i & (1 << b)) == 0) continue;          // this button not pressed
                    presses++;

                    // toggle all lights this button controls
                    foreach (var lightIndex in machine.WiringSchematics[b])
                    {
                        state[lightIndex] = !state[lightIndex];
                    }
                }

                if (state.SequenceEqual(onState) && presses < minPresses)
                {
                    minPresses = presses;
                }
            }

            totalMinPresses += minPresses;
        }

        return totalMinPresses;
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
