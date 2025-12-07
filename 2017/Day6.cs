namespace AOC.AOC2017;

public class Day6 : Day<Day6.Memory>
{
    protected override string? SampleRawInput { get => "0\t2\t7\t0"; }

    private Dictionary<int, List<List<int>>> _cycleLog = [];

    public class Memory
    {
        public List<int> Banks { get; set; } = [];
    }

    protected override Answer Part1()
    {
        RunRoutine();
        return _cycleLog[InputHashCode].Count-1;
    }

    protected override Answer Part2()
    {
        RunRoutine();
        return _cycleLog[InputHashCode].Count - 1 - _cycleLog[InputHashCode].FindIndex(p => p.SequenceEqual(_cycleLog[InputHashCode][^1]));
    }

    private void RunRoutine()
    {
        if (_cycleLog.ContainsKey(InputHashCode)) return;

        _cycleLog[InputHashCode] = [];

        var current = Input.Banks;

        while (!_cycleLog[InputHashCode].Any(p => p.SequenceEqual(current)))
        {
            _cycleLog[InputHashCode].Add([.. current]);

            var max = current.Max();
            var maxIdx = current.IndexOf(max);

            // redistribute
            current[maxIdx] = 0;
            for (var i=0; i<max; i++)
            {
                current[(maxIdx + 1 + i) % current.Count]++;
            }
        }

        // add the final state (for part 2)
        _cycleLog[InputHashCode].Add([.. current]);
    }

    protected override Memory Parse(RawInput input)
    {
        return new Memory
        {
            Banks = [.. input.Lines().First().Split('\t').Select(int.Parse)]
        };
    }
}