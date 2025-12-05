namespace AOC.AOC2025;

public class Day5 : Day<Day5.Ingredients>
{
    protected override string? SampleRawInput { get => "3-5\n10-14\n16-20\n12-18\n\n1\n5\n8\n11\n17\n32"; }

    public class Ingredients
    {
        public required List<(long low, long high)> Fresh { get; set; }
        public required List<long> Available { get; set; }

    }

    protected override Answer Part1()
    {
        var totalAvailable = 0;

        foreach (var avail in Input.Available)
        {
            foreach (var (low, high) in Input.Fresh)
            {
                if (avail >= low && avail <= high)
                {
                    totalAvailable++;
                    break;
                }
            }
        }

        return totalAvailable;
    }

    protected override Answer Part2()
    {
        // start with a single not-fresh range that encompasses the maximum possible fresh range
        var min = Input.Fresh.Min(r => r.low);
        var max = Input.Fresh.Max(r => r.high);

        var notFreshRanges = new List<(long low, long high)>
        {
            (min, max)
        };

        // for each fresh range, remove that range from notFreshRanges, ignoring, trimming, splitting or eliminating according to overlap
        foreach (var (freshLow, freshHigh) in Input.Fresh)
        {
            var newNotFreshRanges = new List<(long low, long high)>();

            foreach (var (notFreshLow, notFreshHigh) in notFreshRanges)
            {
                if (freshLow <= notFreshLow && freshHigh >= notFreshHigh)
                {
                    // fresh range fully covers not-fresh range, eliminate it
                    continue;
                }

                if (freshHigh < notFreshLow || freshLow > notFreshHigh)
                {
                    // no overlap, keep the not-fresh range as-is
                    newNotFreshRanges.Add((notFreshLow, notFreshHigh));
                    continue;
                }

                if (freshLow > notFreshLow && freshHigh < notFreshHigh)
                {
                    // fresh range is fully inside not-fresh range, split into two
                    newNotFreshRanges.Add((notFreshLow, freshLow - 1));
                    newNotFreshRanges.Add((freshHigh + 1, notFreshHigh));
                    continue;
                }

                if (freshLow <= notFreshLow)       // && freshHigh < notFreshHigh
                {
                    // overlap at low end, adjust not-fresh range low up
                    newNotFreshRanges.Add((freshHigh + 1, notFreshHigh));
                    continue;
                }

                if (freshHigh >= notFreshHigh)    // && freshLow > notFreshLow
                {
                    // overlap at high end, adjust not-fresh range high down
                    newNotFreshRanges.Add((notFreshLow, freshLow - 1));
                    continue;
                }

                // should never get here
                throw new Exception("Unhandled range overlap case!");
            }

            notFreshRanges = newNotFreshRanges;
        }

        // total fresh = maximum range with each remaining (disjoint) not-fresh range removed
        return max-min+1 - notFreshRanges.Sum(r => r.high - r.low + 1);
    }

    protected override Ingredients Parse(RawInput input)
    {
        var groups = input.LineGroups();
        var fresh = groups[0].Select(line =>
        {
            var parts = line.Split('-');
            return (long.Parse(parts[0]), long.Parse(parts[1]));
        }).ToList();

        var available = groups[1].Select(long.Parse).ToList();

        return new Ingredients
        {
            Fresh = fresh,
            Available = available
        };
    }
}