namespace AOC.AOC2024;

public class Day11 : Day<Day11.Corridor>
{
    protected override string? SampleRawInput { get => "125 17"; }

    public class Corridor
    {
        public Corridor(List<long> stones)
        {
            Stones = stones;
            ComputeCycleMap();
        }

        public List<long> Stones;

        public Dictionary<long, List<long>> StonesAfterStep = new();        // starting with this single stone -> total number of stones after step i

        public void ComputeCycleMap()
        {
            var cycleMap = new Dictionary<long, List<long>>();              // stone --> stone(s) after one step

            // (only) these values cycle starting with 0.  precompute the number of stones resulting from starting with a single stone of each of these values.
            var check = "8 0 9 6 20 24 3277 2608 3686 9184 2457 9456 4048 1 8096 16192 12144 14168 6072 18216 2024 10120 2 4 32 77 26 36 86 91 84 57 94 56 32772608 40 48 80 96 2867 6032 3 7 5 2048 2880 60 72 28676032 24579456 28 67 36869184 20482880"
                .Split(' ').Select(long.Parse).ToList();

            foreach (var item in check)
            {
                string str;
                if (item == 0)
                {
                    cycleMap[0] = new List<long>() { 1 };
                }
                else if ((str = item.ToString()).Length % 2 == 0)
                {
                    var orig = item;
                    cycleMap[orig] = new List<long>
                    {
                        long.Parse(str[(str.Length / 2)..]),
                        long.Parse(str[..(str.Length / 2)])
                    };
                }
                else
                {
                    cycleMap[item] = new List<long>() { item * 2024 };
                }
                StonesAfterStep.Add(item, new List<long> { cycleMap[item].Count });
            }

            // precompute up to 75
            for (var i=1; i<75; i++)
            {
                foreach (var item in check)
                {
                    var ct = 0L;
                    foreach (var next in cycleMap[item])
                    {
                        ct += StonesAfterStep[next][i-1];
                    }
                    StonesAfterStep[item].Add(ct);
                }
            }
        }
    }

    protected override long Part1()
    {
        return Blink(25);
    }

    protected override long Part2()
    {
        return Blink(75);
    }

    private long Blink(int blinks)
    {
        var ct = 0L;
        foreach (var stone in Input.Stones)
        {
            ct += CountStones(stone, blinks);
        }
        return ct;
    }

    private long CountStones(long stone, int blinksRemaining)
    {
        if (blinksRemaining == 0) return 1;

        // hit a known cycle stone
        if (Input.StonesAfterStep.ContainsKey(stone))
        {
            return Input.StonesAfterStep[stone][blinksRemaining-1];
        }

        // recurse using stone modification rules; ignore the 0 -> 1 rule as zero is already accounted for in the cycle map
        // could get fancy with log here, but this is fast enough.
        string str;
        if ((str = stone.ToString()).Length % 2 == 0)
        {
            return CountStones(long.Parse(str[(str.Length / 2)..]), blinksRemaining-1) +
                CountStones(long.Parse(str[..(str.Length / 2)]), blinksRemaining-1);
        }
        else
        {
            return CountStones(stone * 2024, blinksRemaining-1);
        }        
    }

    protected override Corridor Parse(string input)
    {
        return new Corridor(input.Split(' ').Select(long.Parse).ToList());
    }
}