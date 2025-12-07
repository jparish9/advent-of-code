namespace AOC.AOC2017;

public class Day7 : Day<Day7.Tower>
{
    protected override string? SampleRawInput { get => "pbga (66)\nxhth (57)\nebii (61)\nhavc (66)\nktlj (57)\nfwft (72) -> ktlj, cntj, xhth\nqoyq (66)\npadx (45) -> pbga, havc, qoyq\ntknk (41) -> ugml, padx, fwft\njptl (61)\nugml (68) -> gyxo, ebii, jptl\ngyxo (61)\ncntj (57)"; }

    public class Tower
    {
        public required Dictionary<string, (int Weight, List<string> Supports)> Programs;
    }

    protected override Answer Part1()
    {
        // the program not supported by any other is the bottom program
        var supported = Input.Programs.Values.SelectMany(p => p.Supports).ToHashSet();
        var bottomProgram = Input.Programs.Keys.Except(supported).First();

        return bottomProgram;
    }

    protected override Answer Part2()
    {
        var incorrectProgram = string.Empty;
        var adjustedWeight = 0;
        TotalWeight(Part1().ToString(), ref incorrectProgram, ref adjustedWeight);

        return adjustedWeight;
    }

    private int TotalWeight(string programName, ref string incorrectProgram, ref int adjustedWeight)
    {
        var (weight, supports) = Input.Programs[programName];
        List<int> supportWeights = [];
        foreach (var support in supports)
        {
            supportWeights.Add(TotalWeight(support, ref incorrectProgram, ref adjustedWeight));
        }

        if (incorrectProgram == string.Empty && supportWeights.Distinct().Count() > 1)
        {
            // imbalance detected
            var correctWeight = supportWeights.GroupBy(w => w).OrderByDescending(g => g.Count()).First().Key;
            var incorrectWeight = supportWeights.GroupBy(w => w).OrderBy(g => g.Count()).First().Key;

            incorrectProgram = supports[supportWeights.IndexOf(incorrectWeight)];
            var incorrectProgramCurrentWeight = Input.Programs[incorrectProgram].Weight;
            
            var weightDiff = correctWeight - incorrectWeight;
            adjustedWeight = incorrectProgramCurrentWeight + weightDiff;

            return 0;   // further weights are irrelevant; we have found the imbalance and just need to return out
        }

        return weight + supportWeights.Sum();
    }

    protected override Tower Parse(RawInput input)
    {
        return new Tower
        {
            Programs = input.Lines().Select(line =>
            {
                var parts = line.Split(" -> ");
                var nameWeight = parts[0].Split(' ');
                var name = nameWeight[0];
                var weight = int.Parse(nameWeight[1].Trim('(', ')'));
                var supports = parts.Length > 1 ? [.. parts[1].Split(", ")] : new List<string>();
                return (name, weight, supports);
            }).ToDictionary(p => p.name, p => (p.weight, p.supports))
        };
    }
}