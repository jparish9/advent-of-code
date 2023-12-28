namespace AOC.AOC2015;

public class Day17 : Day<Day17.Refrigerator>
{
    protected override string? SampleRawInput { get => "20\n15\n10\n5\n5"; }

    public class Refrigerator
    {
        public required List<int> Containers { get; set; }
    }

    protected override long Part1()
    {
        return GetCombinations().Count;
    }

    protected override long Part2()
    {
        var combinations = GetCombinations();
        return combinations.Count(p => p.Count == combinations.Min(p => p.Count));
    }

    private List<List<int>> GetCombinations()
    {
        var total = Input.Containers.Count == 5 ? 25 : 150;

        var containers = Input.Containers;

        var combinations = new List<List<int>>();

        // ye olde O(2^n)
        for (var i = 0; i < Math.Pow(2, containers.Count); i++)
        {
            var combination = new List<int>();
            for (var j = 0; j < containers.Count; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    combination.Add(containers[j]);
                }
            }

            if (combination.Sum() == total)
                combinations.Add(combination);
        }

        return combinations;
    }

    protected override Refrigerator Parse(string input)
    {
        return new Refrigerator() { Containers = input.Split("\n").Where(p => p != "").Select(int.Parse).ToList() };
    }
}