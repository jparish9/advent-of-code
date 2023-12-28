namespace AOC.AOC2015;

public class Day16 : Day<Day16.SueFinder>
{
    protected override string? SampleRawInput { get => ""; }

    private Dictionary<string, int> Criteria = new Dictionary<string, int>()
    {
        { "children", 3 },
        { "cats", 7 },
        { "samoyeds", 2 },
        { "pomeranians", 3 },
        { "akitas", 0 },
        { "vizslas", 0 },
        { "goldfish", 5 },
        { "trees", 3 },
        { "cars", 2 },
        { "perfumes", 1 }
    };

    public class SueFinder
    {
        public required List<Sue> Sues { get; set; }
    }

    public class Sue
    {
        public int Number { get; set; }
        public Dictionary<string, int> Properties { get; set; } = new Dictionary<string, int>();

        public bool Matches(Dictionary<string, int> criteria)
        {
            foreach (var p in Properties)
            {
                if (criteria.ContainsKey(p.Key) && criteria[p.Key] != p.Value)
                    return false;
            }

            return true;
        }

        public bool Matches2(Dictionary<string, int> criteria)
        {
            foreach (var property in Properties)
            {
                if (criteria.ContainsKey(property.Key))
                {
                    if (property.Key == "cats" || property.Key == "trees")
                    {
                        if (criteria[property.Key] >= property.Value)
                            return false;
                    }
                    else if (property.Key == "pomeranians" || property.Key == "goldfish")
                    {
                        if (criteria[property.Key] <= property.Value)
                            return false;
                    }
                    else if (criteria[property.Key] != property.Value)
                        return false;
                }
            }

            return true;
        }
    }

    protected override long Part1()
    {
        if (Input == null || Input.Sues.Count == 0) return 0;           // no sample

        return Input.Sues.First(p => p.Matches(Criteria)).Number;
    }

    protected override long Part2()
    {
        if (Input == null || Input.Sues.Count == 0) return 0;           // no sample

        return Input.Sues.First(p => p.Matches2(Criteria)).Number;
    }

    protected override SueFinder Parse(string input)
    {
        var sues = new List<Sue>();

        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            var parts = line.Split(' ');
            var number = int.Parse(parts[1].Trim(':'));
            var sue = new Sue() { Number = number };

            for (int i = 2; i < parts.Length; i += 2)
            {
                var property = parts[i].Trim(':');
                var value = int.Parse(parts[i + 1].Trim(','));
                sue.Properties.Add(property, value);
            }

            sues.Add(sue);
        }

        return new SueFinder() { Sues = sues };
    }
}