namespace AOC.AOC2015;

public class Day5 : Day<List<Day5.MaybeNice>>
{
    protected override string? SampleRawInput { get => "qjhvhtzxzqqjkmpb"; }

    public class MaybeNice
    {
        public required string Word { get; set; }

        private static readonly char[] vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        private static readonly string[] badStrings = new[] { "ab", "cd", "pq", "xy" };

        public bool IsNice()
        {
            var vowelCount = Word.Count(c => vowels.Contains(c));
            if (vowelCount < 3) return false;

            var doubles = Word.Select((c, i) => (c, i)).Where(p => p.i < Word.Length - 1 && p.c == Word[p.i + 1]).Count();
            if (doubles == 0) return false;

            return !badStrings.Any(s => Word.Contains(s));
        }

        public bool IsNicer()
        {
            var pairs = Word.Select((c, i) => (c, i)).Where(p => p.i < Word.Length - 1).Select(p => (Word.Substring(p.i, 2), p.i)).ToList();
            var hasPair = pairs.Any(p => pairs.Any(p2 => p2.Item1.Equals(p.Item1) && Math.Abs(p2.i-p.i) > 1));

            var hasRepeat = Word.Select((c, i) => (c, i)).Where(p => p.i < Word.Length - 2 && p.c == Word[p.i + 2]).Any();
            return hasPair && hasRepeat;
        }
    }

    protected override long Part1()
    {
        return Input.Count(w => w.IsNice());
    }

    protected override long Part2()
    {
        return Input.Count(w => w.IsNicer());
    }

    protected override List<MaybeNice> Parse(string input)
    {
        var list = new List<MaybeNice>();
        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            list.Add(new MaybeNice() { Word = line });
        }

        return list;
    }
}