namespace AOC.AOC2017;

public class Day4 : Day<Day4.Passphrases>
{
    protected override string? SampleRawInput { get => "aa bb cc dd ee\naa bb cc dd aa\naa bb cc dd aaa"; }
    protected override string? SampleRawInputPart2 { get => "abcde fghij\nabcde xyz ecdab\na ab abc abd abf abj\niiii oiii ooii oooi oooo\noiii ioii iioi iiio"; }

    public class Passphrases
    {
        public required List<string> Phrases;
    }

    protected override Answer Part1()
    {
        return Input.Phrases.Count(p => p.Split(" ").GroupBy(q => q).All(r => r.Count() == 1));
    }

    protected override Answer Part2()
    {
        return Input.Phrases.Count(p => p.Split(" ").GroupBy(q => new string([.. q.OrderBy(r => r)])).All(r => r.Count() == 1));            // O_o
    }

    protected override Passphrases Parse(string input)
    {
        return new Passphrases() { Phrases = input.Split("\n").Where(p => p != "").ToList() };
    }
}