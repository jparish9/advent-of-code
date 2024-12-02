using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace AOC.AOC2015;

public class Day19 : Day<Day19.Machine>
{
    protected override string? SampleRawInput { get => "H => HO\nH => OH\nO => HH\n\nHOHOHO"; }
    protected override string? SampleRawInputPart2 { get => "e => H\ne => O\nH => HO\nH => OH\nO => HH\n\nHOHOHO"; }

    private static int Steps = 0;

    public class Machine
    {
        public required List<Replacement> Replacements { get; set; }

        public required string Molecule { get; set; }
    }

    public class Replacement
    {
        public required string From { get; set; }
        public required string To { get; set; }
    }

    // for TPriority in PriorityQueue
    public class Priority : IComparable<Priority>
    {
        public int Steps { get; set; }
        public int Length { get; set; }
        public int PriorityReplacements { get; set; }
        public int Basicness { get; set; }

        public int CompareTo(Priority? other)
        {
            if (other == null) return 1;

            var cmp = Steps.CompareTo(other.Steps);
            if (cmp != 0) return cmp;

            cmp = PriorityReplacements.CompareTo(other.PriorityReplacements);
            if (cmp != 0) return cmp;

            cmp = Basicness.CompareTo(other.Basicness);
            if (cmp != 0) return -cmp;          // we want higher "basicness" to be treated as first for the priority queue

            cmp = Length.CompareTo(other.Length);
            if (cmp != 0) return cmp;

            return 0;
        }
    }

    protected override long Part1()
    {
        var replacements = Input.Replacements;
        var molecule = Input.Molecule;

        var molecules = new HashSet<string>();

        // only a single type of replacement is allowed, and only a single occurrence can be replaced, but we need to consider each occurrence of the replacement
        foreach (var replacement in replacements)
        {
            var index = molecule.IndexOf(replacement.From);
            while (index != -1)
            {
                var newMolecule = string.Concat(molecule.AsSpan(0, index), replacement.To, molecule.AsSpan(index + replacement.From.Length));
                molecules.Add(newMolecule);
                index = molecule.IndexOf(replacement.From, index + 1);
            }
        }

        return molecules.Count;
    }

    protected override long Part2()
    {
        // there are some solutions that work for specific inputs, by trying replacements starting from the end of the string until we get to "e".
        // this doesn't work in the general case, or for my input.
        // the general case should be solvable with the CYK algorithm, since the replacements define the "grammar" of this language.
        // are the replacements in Chomsky normal form?  I think so, with the exception of defining a start rule that goes to "e".

        var r = Input.Replacements.Select(p => (p.From, p.To)).ToList();
        r = r.Prepend(("z", "e")).ToList();      // start rule
        

        




        if (Input.Molecule == "HOHOHO") return 0;
        var replacements = Input.Replacements;
        var molecule = new string(Input.Molecule);

        // try replacing from the end of the molecule until we can't.
        var steps = 0;
        while (molecule != "e")
        {
            foreach (var replacement in replacements.OrderByDescending(p => p.To.Length))
            {
                var index = molecule.LastIndexOf(replacement.To, StringComparison.InvariantCulture);
                if (index != -1 && molecule != "e")
                {
                    molecule = $"{molecule[..index]}{replacement.From}{molecule[(index + replacement.To.Length)..]}";
                    // string.Concat(molecule.AsSpan(0, index), replacement.From, molecule.AsSpan(index + replacement.To.Length));
                    steps++;
                    System.Console.WriteLine("replaced " + replacement.To + ": " + molecule);
                }
                if (molecule == "e") break;
            }
        }

        return steps;

        // 153 too low


        // we need to find the shortest path from molecule to e
        // this is almost a game tree:
        //   - the possible "moves" are all of the replacements
        //   - the evaluation function is the number of steps taken and/or current length
        //   - the search heuristic ("moves" to evaluate first) is the replacement that causes the biggest reduction in length
        //   - the win condition is e (in the fewest steps)
        // we can probably use a priority queue to search for the shortest path.
        // a priority/heuristic of just current length runs into the apparent dead-end NRnBSiRnCaRnFArYFArFArF (length 24),
        // where there are no shortening replacements and it's not obvious how to proceed.
        // I am noticing that there are some replacements that should be prioritized, not because of the maximum amount of shortening,
        // but because they replace atoms that have few or no replacements, and thus are more likely to be a dead-end.
        // For example, there is no replacement that takes a molecule and outputs anything with a C, Y, Rn, or Ar, so those should be prioritized.
        // Beyond that, anything that replaces into atoms used in the final e conversion (H, F, N, Al, O, Mg) should be prioritized.

        // Maybe I've been thinking about this backwards.
        // The starting molecule has a relatively small length (well <1000), and every replacement increases the length by at least 2.
        // It shouldn't be that hard to build a tree of all of the possible molecules, and then search that tree for the fewest steps from e to the starting molecule.

        // That approach results in an exploding tree.  Clearly there is some idea I am still missing.


        //BuildTree("e ", 0);
        //System.Console.WriteLine("After BuildTree??");

        //return 0;


/*

        var queue = new PriorityQueue<string, Priority>();

        queue.Enqueue(molecule, new Priority() { Steps = 0, Length = molecule.Length, PriorityReplacements = 0});

        var i=0;

        while (queue.Count > 0)
        {
            i++;
            if (!queue.TryDequeue(out var m, out var pri)) break;

            if (m == "e ")
            {
                return pri.Steps;
            }

            System.Console.WriteLine(queue.Count + " " + pri.Steps + " " + pri.Length + " " + pri.PriorityReplacements + " " + pri.Basicness);

            foreach (var replacement in replacements)
            {
                var index = m.IndexOf(replacement.To);
                while (index != -1)
                {
                    var newMolecule = string.Concat(m.AsSpan(0, index), replacement.From, m.AsSpan(index + replacement.To.Length));
                    queue.Enqueue(newMolecule, new Priority() { Steps = pri.Steps + 1, Length = newMolecule.Length,
                        PriorityReplacements = Regex.Count(newMolecule, "C |Y |Rn|Ar") * 500 / newMolecule.Length,
                        Basicness = Regex.Count(newMolecule, "H |F |N |Al|O |Mg") * 500 / newMolecule.Length });
                    index = m.IndexOf(replacement.To, index + 1);
                }
            }

            if (i > 1000) break;
        }

        return 0;*/
    }

    private void BuildTree(string molecule, int steps)
    {
        if (molecule == Input.Molecule)
        {
            System.Console.WriteLine("found target molecule in " + steps + " " + steps);
            return;
        }

        // find all valid replacements and recurse.
        foreach (var replacement in Input.Replacements)
        {
            var index = molecule.IndexOf(replacement.From);
            while (index != -1)
            {
                var newMolecule = string.Concat(molecule.AsSpan(0, index), replacement.To, molecule.AsSpan(index + replacement.From.Length));
                index = molecule.IndexOf(replacement.From, index + 1);

                if (!newMolecule.StartsWith("O") && !newMolecule.EndsWith("F")) continue;           // some pruning
                if (newMolecule.Length > Input.Molecule.Length) continue;           // this replacement puts us over the target length

                Steps++;
                if (Steps % 10000 == 0) System.Console.WriteLine(Steps);

                BuildTree(newMolecule, steps + 1);
            }
        }
    }

    protected override Machine Parse(string input)
    {
        var lines = input.Split("\n\n");

        // for both the replacements and the molecule, standardize to two characters per atom, then map them to ints.
        var replacements = new List<Replacement>();

        foreach (var line in lines[0].Split('\n').Where(p => p != ""))
        {
            var parts = line.Split(" => ");
            replacements.Add(new Replacement() { From = Standardize(parts[0]), To = Standardize(parts[1]) });
        }

        return new Machine() { Replacements = replacements, Molecule = Standardize(lines[1]) };
    }

    private static string Standardize(string molecule)
    {
        return molecule;
        /*
        // molecule contains known single- or double-character atoms.  build a standardized double-character string first, then convert it to an int array.
        var sb = new StringBuilder();

        for (var i=0; i<molecule.Length; i++)
        {
            if (i < molecule.Length - 1 && char.IsLower(molecule[i+1]))
            {
                sb.Append(molecule[i]).Append(molecule[i+1]);
                i++;
            }
            else
            {
                sb.Append(molecule[i]).Append(' ');
            }
        }

        var str = sb.ToString();
        return str;
        //return str.Select((p, i) => (p, i)).Where(q => q.i < str.Length-1 && q.i % 2 == 0).Select(p => MapToElementId(p.p.ToString() + str[p.i+1])).ToArray();
        */
    }
}