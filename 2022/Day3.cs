namespace AOC.AOC2022;

public class Day3 : Day<List<string>>
{
    protected override string? SampleRawInput { get => "vJrwpWtwJgWrhcsFMMfFFhFp\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\nPmmdzqPrVvPwwTWBwg\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\nttgJtRGJQctTZtZT\nCrZsJsPPZsGzwwsLwLmpwMDw"; }
    // public override bool Part2ParsedDifferently => true;

    protected override Answer Part1()
    {
        var result = new List<char>();

        foreach (var line in Input)
        {
            HashSet<char> left = new();
            line[..(line.Length / 2)].ToList().ForEach(p => left.Add(p));

            // find the only character in the right half that is also in the left half
            result.Add(line[(line.Length / 2)..].First(left.Contains));
        }

        return GetPrioritySum(result);
    }

    protected override Answer Part2()
    {
        var result = new List<char>();

        for (var i=0; i<Input.Count; i+=3)
        {
            HashSet<char> inAll = new();
            Input[i].ToList().ForEach(p => inAll.Add(p));

            for (var j=i+1; j<i+3; j++)
            {
                HashSet<char> inThis = new();
                Input[j].ToList().ForEach(p => inThis.Add(p));

                inAll.IntersectWith(inThis);
            }

            result.Add(inAll.First());
        }

        return GetPrioritySum(result);
    }

    private static long GetPrioritySum(List<char> result)
    {
        return result.Sum(p => p >= 'a' ? p - 'a' + 1 : p - 'A' + 27);
    }

    protected override List<string> Parse(string input)
    {
        return input.Split('\n').Where(p => p != "").ToList();
    }
}