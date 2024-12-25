using AOC.Utils;

namespace AOC.AOC2024;

public class Day25 : Day<Day25.Door>
{
    protected override string? SampleRawInput { get => "#####\n.####\n.####\n.####\n.#.#.\n.#...\n.....\n\n#####\n##.##\n.#.##\n...##\n...#.\n...#.\n.....\n\n.....\n#....\n#....\n#...#\n#.#.#\n#.###\n#####\n\n.....\n.....\n#.#..\n###..\n###.#\n###.#\n#####\n\n.....\n.....\n.....\n#....\n#.#..\n#.#.#\n#####"; }

    public class Door
    {
        public required List<Lock> Locks;
        public required List<Key> Keys;
    }

    public class Lock
    {
        public required List<int> PinHeights;
    }

    public class Key
    {
        public required List<int> KeyHeights;

        public bool FitsLock(Lock lk)
        {
            return KeyHeights.Zip(lk.PinHeights).All(p => p.First + p.Second <= 5);
        }
    }

    protected override Answer Part1()
    {
        var ct = 0;
        foreach (var i in Input.Keys)
        {
            foreach (var j in Input.Locks)
            {
                if (i.FitsLock(j)) ct++;
            }
        }

        return ct;
    }

    protected override Answer Part2()
    {
        return "Merry Christmas!";              // there is no part 2!
    }

    protected override Door Parse(RawInput input)
    {
        var locksAndKeys = input.LineGroups();

        var locks = new List<Lock>();
        var keys = new List<Key>();

        foreach (var item in locksAndKeys)
        {
            // convert to columns for pin/key heights
            var cols = item.SelectMany(p => p.Select((c, i) => new { c, i })).GroupBy(p => p.i).Select(p => p.Select(q => q.c).ToList()).ToList();
            var heights = cols.Select(p => p.Count(q => q == '#') - 1).ToList();

            // locks have top row of #####, keys have bottom row of #####
            if (item.First() == "#####")
            {
                locks.Add(new Lock() { PinHeights = heights });
            }
            else if (item.Last() == "#####")
            {
                keys.Add(new Key() { KeyHeights = heights });
            }
            else
            {
                throw new Exception("Invalid input");
            }
        }

        return new Door() { Locks = locks, Keys = keys };
    }
}