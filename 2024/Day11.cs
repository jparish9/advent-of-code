namespace AOC.AOC2024;

public class Day11 : Day<Day11.Corridor>
{
    protected override string? SampleRawInput { get => "125 17"; }

    public class Corridor
    {
        public required List<long> Stones;
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
        // the order of the stones does not matter.  we can just keep a dictionary of stone -> count, and update the counts en masse by applying the stone modification rules.
        var dict = new Dictionary<long, long>();                  // stone -> count
        var next = new Dictionary<long, List<long>>();            // stone -> next stone
        foreach (var stone in Input.Stones)
        {
            dict[stone] = 1;
        }

        for (var i=0; i<blinks; i++)
        {
            var mod = new Dictionary<long, long>();         // store modifications to counts and apply them at the end of each blink
            foreach (var stone in dict.Where(p => p.Value > 0).Select(p => p.Key))
            {
                // cache the modification rule
                if (!next.ContainsKey(stone))
                {
                    string str;
                    if (stone == 0)
                    {
                        next[stone] = new List<long>() { 1 };
                    }
                    else if ((str = stone.ToString()).Length % 2 == 0)
                    {
                        next[stone] = new List<long>
                        {
                            long.Parse(str[(str.Length / 2)..]),
                            long.Parse(str[..(str.Length / 2)])
                        };
                    }
                    else
                    {
                        next[stone] = new List<long>() { stone * 2024 };
                    }
                }

                foreach (var nextStone in next[stone])
                {
                    if (!mod.ContainsKey(nextStone))
                        mod[nextStone] = 0;
                    mod[nextStone] += dict[stone];
                }

                if (!mod.ContainsKey(stone))
                    mod[stone] = 0;
                mod[stone] -= dict[stone];
            }

            // apply changes
            foreach (var key in mod.Keys)
            {
                if (!dict.ContainsKey(key))
                    dict[key] = 0;
                dict[key] += mod[key];
            }
        }

        return dict.Values.Sum();
    }

    protected override Corridor Parse(string input)
    {
        return new Corridor() { Stones = input.Split(' ').Select(long.Parse).ToList() };
    }
}