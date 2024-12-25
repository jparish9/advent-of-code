namespace AOC.AOC2024;

public class Day19 : Day<Day19.Onsen>
{
    protected override string? SampleRawInput { get => "r, wr, b, g, bwu, rb, gb, br\n\nbrwrr\nbggr\ngbbr\nrrbgbr\nubwu\nbwurrg\nbrgr\nbbrgwb"; }

    public class Onsen
    {
        public required List<string> Patterns;
        public required List<string> Designs;
        public Dictionary<string, long> DesignFragments = [];           // cache
        public Dictionary<string, long> DesignCounts = [];              // cache

        public void CheckAll()
        {
            DesignCounts.Clear();
            foreach (var design in Designs)
            {
                DesignCounts.Add(design, CanMakeDesign(design));
            }
        }

        public long CanMakeDesign(string design)
        {
            var valid = 0L;
            foreach (var pattern in Patterns)
            {
                if (design.StartsWith(pattern))
                {
                    var newDesign = design[pattern.Length..];
                    // recurse remaining design, or count valid if nothing left
                    if (newDesign != "")
                    {
                        // check cache!  only recurse if not found
                        if (!DesignFragments.TryGetValue(newDesign, out var ct))
                        {
                            ct = CanMakeDesign(newDesign);
                            DesignFragments.Add(newDesign, ct);
                        }
                        valid += ct;
                    }
                    else valid++;
                }
            }

            return valid;
        }
    }

    protected override Answer Part1()
    {
        Input.CheckAll();
        return Input.DesignCounts.Where(p => p.Value > 0).Count();
    }

    protected override Answer Part2()
    {
        return Input.DesignCounts.Sum(p => p.Value);            // cached from part 1
    }

    protected override Onsen Parse(string input)
    {
        var parts = input.Split("\n\n");
        return new Onsen()
        {
            Patterns = [.. parts[0].Split(", ")],
            Designs = parts[1].Split("\n").Where(p => p != "").ToList()
        };
    }
}