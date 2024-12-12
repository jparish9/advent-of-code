namespace AOC.AOC2020;

public class Day6 : Day<List<Day6.Answers>>
{
    protected override string? SampleRawInput { get => "abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb"; }

    public class Answers
    {
        public required List<string> Person { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.Sum(p => p.Person.SelectMany(p => p).Distinct().Count());
    }

    protected override Answer Part2()
    {
        return Input.Sum(p => p.Person.SelectMany(p => p).Distinct().Count(q => p.Person.All(r => r.Contains(q))));
    }

    protected override List<Answers> Parse(string input)
    {
        return input.Split("\n\n").Where(p => p != "").Select(p => {
            return new Answers {
                Person = p.Split('\n').Where(p => p != "").ToList()
            };
        }).ToList();
    }
}