namespace AOC.AOC2017;

public class Day10 : Day<Day10.KnotHash>
{
    protected override string? SampleRawInput { get => "3,4,1,5"; }
    protected override string? SampleRawInputPart2 { get => "AoC 2017"; }

    public class KnotHash
    {
        public required List<int> Lengths { get; set; }
        public List<int> CurrentList { get { return _currentList; }
            set
            {
                // resetting current list also resets state variables
                _currentList = value;
                LastPosition = 0;
                LastSkipSize = 0;
            }
        }
        public int LastPosition { get; set; } = 0;
        public int LastSkipSize { get; set; } = 0;

        private List<int> _currentList = [];
    }

    protected override Answer Part1()
    {
        Input.CurrentList = [.. Enumerable.Range(0, IsSampleInput ? 5 : 256)];

        RunRound();

        return Input.CurrentList[0] * Input.CurrentList[1];
    }

    protected override Answer Part2()
    {
        Input.CurrentList = [.. Enumerable.Range(0, 256)];

        for (var i=0; i<64; i++)
        {
            RunRound();
        }

        var denseHash = new List<int>();
        for (var block=0; block < 16; block++)
        {
            var xor = 0;
            for (var i=0; i<16; i++)
            {
                xor ^= Input.CurrentList[block * 16 + i];
            }
            denseHash.Add(xor);
        }

        return string.Concat(denseHash.Select(b => b.ToString("x2")));          // format each dense hash number (0-255) as 2-digit hex, then concanenate
    }

    private void RunRound()
    {
        var listPos = Input.LastPosition;
        var skipSize = Input.LastSkipSize;
        var listSize = Input.CurrentList.Count;
        
        for (var lengthPos = 0; lengthPos < Input.Lengths.Count; lengthPos++)
        {
            var length = Input.Lengths[lengthPos];

            // reverse section
            var endPos = listPos + length - 1;
            for (var i=0; i < length / 2; i++)
            {
                (Input.CurrentList[(endPos - i) % listSize], Input.CurrentList[(listPos + i) % listSize]) = (Input.CurrentList[(listPos + i) % listSize], Input.CurrentList[(endPos - i) % listSize]);
            }

            listPos = (listPos + length + lengthPos + skipSize) % listSize;        // advance by current length + skip size
        }

        Input.LastPosition = listPos;
        Input.LastSkipSize = skipSize + Input.Lengths.Count;
    }

    protected override KnotHash Parse(RawInput input)
    {
        var line = input.Lines().Count != 0 ? input.Lines()[0] : "";        // allow testing with empty string as sample input

        var lengths = IsPart2 ? line.Select(c => (int)c).Concat([17, 31, 73, 47, 23])
            : line.Split(',').Select(int.Parse);
        return new KnotHash() { Lengths = [.. lengths] };
    }
}