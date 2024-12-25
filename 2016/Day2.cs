namespace AOC.AOC2016;

public class Day2 : Day<Day2.KeyPad>
{
    protected override string? SampleRawInput { get => "ULL\nRRDDD\nLURDL\nUUUUD"; }

    public class KeyPad
    {
        private static readonly Dictionary<string, string[]> _part1Map = new()
        {
            { "1", new[] { "1", "2", "4", "1" } },
            { "2", new[] { "2", "3", "5", "1" } },
            { "3", new[] { "3", "3", "6", "2" } },
            { "4", new[] { "1", "5", "7", "4" } },
            { "5", new[] { "2", "6", "8", "4" } },
            { "6", new[] { "3", "6", "9", "5" } },
            { "7", new[] { "4", "8", "7", "7" } },
            { "8", new[] { "5", "9", "8", "7" } },
            { "9", new[] { "6", "9", "9", "8" } }
        };

        private static readonly Dictionary<string, string[]> _part2Map = new()
        {
            { "1", new[] { "1", "1", "3", "1" } },
            { "2", new[] { "2", "3", "6", "2" } },
            { "3", new[] { "1", "4", "7", "2" } },
            { "4", new[] { "4", "4", "8", "3" } },
            { "5", new[] { "5", "6", "5", "5" } },
            { "6", new[] { "2", "7", "A", "5" } },
            { "7", new[] { "3", "8", "B", "6" } },
            { "8", new[] { "4", "9", "C", "7" } },
            { "9", new[] { "9", "9", "9", "8" } },
            { "A", new[] { "6", "B", "A", "A" } },
            { "B", new[] { "7", "C", "D", "A" } },
            { "C", new[] { "8", "C", "C", "B" } },
            { "D", new[] { "B", "D", "D", "D" } }
        };

        private static readonly string _dirMap = "URDL";            // map to index in _map above

        public required List<string> Instructions { get; set; }

        public string GetCode(bool part2 = false)
        {
            var code = "";
            var pos = "5";

            var map = part2 ? _part2Map : _part1Map;

            foreach (var instruction in Instructions)
            {
                foreach (var dir in instruction)
                {
                    var ndx = _dirMap.IndexOf(dir);
                    pos = map[pos][ndx];
                }
                code += pos;
            }

            return code;
        }
    }

    protected override Answer Part1()
    {
        return int.Parse(Input.GetCode());
    }

    protected override Answer Part2()
    {
        System.Console.WriteLine(Input.GetCode(true));          // string answer
        return 0;
    }

    protected override KeyPad Parse(RawInput input)
    {
        return new KeyPad() { Instructions = input.Lines().ToList() };
    }
}