namespace AOC.AOC2017;

public class Day9 : Day<Day9.Stream>
{
    protected override string? SampleRawInput { get => "{}\n{{{}}}\n{{},{}}\n{{{},{},{{}}}}\n{<a>,<a>,<a>,<a>}\n{{<ab>},{<ab>},{<ab>},{<ab>}}\n{{<!!>},{<!!>},{<!!>},{<!!>}}\n{{<a!>},{<a!>},{<a!>},{<ab>}}"; }

    protected override string? SampleRawInputPart2 { get => "<>\n<random characters>\n<<<<>\n<{!>}>\n<!!>\n<!!!>>\n<{o\"i!a,<{i<a>"; }

    public class Stream
    {
        public List<string> Lines { get; set; } = [];

        public Dictionary<int, Cache> Cache { get; set; } = [];
    }

    public class Cache
    {
        public int TotalScore { get; set; }
        public int TotalGarbage { get; set; }
    }

    protected override Answer Part1()
    {
        Scan();
        return Input.Cache[InputHashCode].TotalScore;

    }

    protected override Answer Part2()
    {
        Scan();
        return Input.Cache[InputHashCode].TotalGarbage;
    }

    private void Scan()
    {
        if (Input.Cache.ContainsKey(InputHashCode)) return;         // already run for this input

        var totalScore = 0;
        var totalGarbage = 0;

        foreach (var line in Input.Lines)
        {
            var lineScore = 0;
            var lineGarbage = 0;
            var depth = 0;                  // current depth of nested {}
            var inGarbage = false;          // currently in garbage? (<..>)
            var exclam = false;             // last character was ! => skip next char

            foreach (var ch in line)
            {
                if (exclam)
                {
                    exclam = false;
                    continue;
                }

                if (ch == '!') exclam = true;
                else if (inGarbage)
                {
                    if (ch == '>') inGarbage = false;       // end of garbage
                    else lineGarbage++;                     // count garbage characters
                }
                else
                {
                    if (ch == '<') inGarbage = true;
                    else if (ch == '{') depth++;
                    else if (ch == '}')
                    {
                        lineScore += depth;         // completed a [nested] group
                        depth--;
                    }
                }
            }

            totalScore += lineScore;
            totalGarbage += lineGarbage;
        }

        Input.Cache[InputHashCode] = new Cache() { TotalGarbage = totalGarbage, TotalScore = totalScore };
    }

    protected override Stream Parse(RawInput input)
    {
        return new Stream
        {
            Lines = [.. input.Lines()]
        };
    }
}