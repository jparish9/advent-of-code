namespace AOC.AOC2020;

public class Day2 : Day<List<Day2.Password>>
{
    protected override string? SampleRawInput { get => "1-3 a: abcde\n1-3 b: cdefg\n2-9 c: ccccccccc"; }
    // protected override bool Part2ParsedDifferently => true;

    public class Password
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public char Char { get; set; }
        public required string Pwd { get; set; }
    }

    protected override Answer Part1()
    {
        var valid = 0;

        foreach (var pwd in Input)
        {
            var count = pwd.Pwd.Count(p => p == pwd.Char);
            if (count >= pwd.Min && count <= pwd.Max)
            {
                valid++;
            }
        }

        return valid;
    }

    protected override Answer Part2()
    {
        // each password must contain the char at exactly one of the two positions
        return Input.Count(p => p.Pwd[p.Min-1] == p.Char ^ p.Pwd[p.Max-1] == p.Char);
    }

    protected override List<Password> Parse(RawInput input)
    {
        return input.Lines().Select(p => {
            var parts = p.Split(' ');
            var minMax = parts[0].Split('-');
            return new Password {
                Min = int.Parse(minMax[0]),
                Max = int.Parse(minMax[1]),
                Char = parts[1][0],
                Pwd = parts[2]
            };
        }).ToList();
    }
}