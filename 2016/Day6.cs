namespace AOC.AOC2016;

public class Day6 : Day<Day6.Message>
{
    protected override string? SampleRawInput { get => "eedadn\ndrvtee\neandsr\nraavrd\natevrs\ntsrnev\nsdttsa\nrasrtv\nnssdts\nntnada\nsvetve\ntesnvt\nvntsnd\nvrdear\ndvrsen\nenarar"; }

    public class Message
    {
        public required List<string> Lines;
        public List<(char min, char max)> MinMaxFreqs = new();

        public void AnalyzeFrequencies()
        {
            MinMaxFreqs.Clear();
            var len = Lines[0].Length;
            var decoded = new char[len];

            for (var i=0; i<len; i++)
            {
                var freq = new Dictionary<char, int>();
                foreach (var line in Lines)
                {
                    var c = line[i];
                    if (!freq.ContainsKey(c)) freq[c] = 0;
                    freq[c]++;
                }

                MinMaxFreqs.Add((freq.First(p => p.Value == freq.Values.Min()).Key, freq.First(p => p.Value == freq.Values.Max()).Key));
            }
        }
    }

    protected override Answer Part1()
    {
        Input.AnalyzeFrequencies();
        return new string(Input.MinMaxFreqs.Select(p => p.max).ToArray());
    }

    protected override Answer Part2()
    {
        return new string(Input.MinMaxFreqs.Select(p => p.min).ToArray());
    }

    protected override Message Parse(string input)
    {
        return new Message() { Lines = input.Split("\n").Where(p => p != "").ToList() };
    }
}