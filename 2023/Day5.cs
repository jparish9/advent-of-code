namespace AOC.AOC2023;

public class Day5 : Day<Day5.Garden>
{
    protected override string? SampleRawInput { get => "seeds: 79 14 55 13\n\nseed-to-soil map:\n50 98 2\n52 50 48\n\nsoil-to-fertilizer map:\n0 15 37\n37 52 2\n39 0 15\n\nfertilizer-to-water map:\n49 53 8\n0 11 42\n42 0 7\n57 7 4\n\nwater-to-light map:\n88 18 7\n18 25 70\n\nlight-to-temperature map:\n45 77 23\n81 45 19\n68 64 13\n\ntemperature-to-humidity map:\n0 69 1\n1 0 69\n\nhumidity-to-location map:\n60 56 37\n56 93 4"; }

    public class Garden
    {
        public required List<Seed> Seeds { get; set; }

        public required List<List<Map>> Maps { get; set; }
    }

    public class Seed
    {
        public long Id { get; set; }
        public long Range { get; set; }
    }

    public class Map
    {
        public long Src { get; set; }
        public long Dest { get; set; }
        public long Range { get; set; }
    }

    private class Range
    {
        public int BeforeStep { get ; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }

    protected override long Part1()
    {
        // true copy
        var seeds = new List<Seed>();
        foreach (var seed in Input.Seeds)
        {
            seeds.Add(new Seed() { Id = seed.Id });     // don't use range for part 1
        }

        foreach (var seed in seeds)
        {
            foreach (var map in Input.Maps)
            {
                foreach (var mapItem in map)
                {
                    if (seed.Id >= mapItem.Src && seed.Id <= mapItem.Src + mapItem.Range)
                    {
                        seed.Id = mapItem.Dest - (mapItem.Src - seed.Id);
                        break;
                    }
                }
            }
        }

        return seeds.Min(p => p.Id);
    }

    protected override long Part2()
    {
        // copy seed definitions to initial ranges
        var ranges = new List<Range>();
        foreach (var seed in Input.Seeds)
        {
            ranges.Add(new Range() { Start = seed.Id, End = seed.Id + seed.Range-1, BeforeStep = 1 });
        }

        for(var step=1; step <= Input.Maps.Count; step++)
        {
            foreach (var range in ranges.Where(p => p.BeforeStep == step).ToList())
            {
                var sortedMaps = Input.Maps[step-1].OrderBy(p => p.Src).ToList();

                var rangeStart = range.Start;
                var rangeEnd = range.End;

                var i=0;

                // Handle all the possibilities of overlapping/disjoint/engulfing maps for this range.
                // Add a range for the next step (mapped or pass-through), reduce the current range as needed, and continue until the current range is disjoint with all maps, or has been completely mapped.
                // Makes use of sorted maps, and that the maps are disjoint with each other.

                while (true)
                {
                    var map = sortedMaps[i];
                    if (rangeStart < map.Src)
                    {
                        if (rangeEnd < map.Src)
                        {
                            /**
                                RANGE :    [<------>]
                                MAP   :                 [<------>]
                                ADD   :    [<------>] (pass through)

                                our working range is entirely below the current map, so pass it through and stop iterating maps for this range because they're sorted.
                            **/
                            ranges.Add(new Range() { Start = rangeStart, End = rangeEnd, BeforeStep = step+1 });
                            break;
                        }
                        /**
                            RANGE    :    [<------->]
                            MAP      :          [<------>]
                            ADD      :    [<-->] (pass through)
                            NEW RANGE:          [<->]

                            add a passthrough range from the range start to the map's start, adjust the working range, and continue
                        **/
                        ranges.Add(new Range() { Start = rangeStart, End = map.Src - 1, BeforeStep = step+1 });
                        rangeStart = map.Src;
                    }
                    else if (rangeStart >= map.Src && rangeStart < map.Src + map.Range)
                    {
                        var offset = map.Dest - map.Src;
                        if (rangeEnd < map.Src + map.Range)
                        {
                            /**
                                RANGE :     [<--->]
                                MAP   :    [<------>]
                                ADD   :     [<--->] (mapped)

                                this map entirely engulfs the range, so add it (completely mapped) and stop iterating maps for this range (this range is now completely handled).
                            **/
                            ranges.Add(new Range() { Start = rangeStart + offset, End = rangeEnd + offset, BeforeStep = step+1 });
                            break;
                        }
                        /**
                            RANGE    :      [<----->]
                            MAP      :   [<---->]
                            ADD      :      [<->]  (mapped)
                            NEW RANGE:           [<>]

                            add a mapped range up to the map's end, adjust the range start, and continue 
                        **/
                        ranges.Add(new Range() { Start = rangeStart + offset, End = map.Dest + map.Range, BeforeStep = step+1 });
                        rangeStart = map.Src + map.Range;
                    }
                    else if (rangeStart >= map.Src + map.Range)
                    {
                        /**
                            RANGE    :            [<----->]
                            MAP      :   [<--->]
                        
                            our working range is entirely above the map, since the maps are sorted we ignore it and move to the next one.
                            note that we do NOT add a pass-through here, because the next map(s) may be in the range.
                        **/
                        i++;
                        if (i == sortedMaps.Count) break;
                    }
                }

                if (!ranges.Any(p => p.BeforeStep == step+1))
                {
                    /**
                        RANGE    :                  [<--->]
                        ALL MAPS :  [<--------->]
                        ADD      :                  [<--->]  (pass through)

                        if no ranges were added, it means all maps were below the range, pass through the full original range to the next step
                    **/
                    ranges.Add(new Range() { Start = range.Start, End = range.End, BeforeStep = step+1});
                    break;
                }  
            }
        }

        return ranges.Where(p => p.BeforeStep == Input.Maps.Count+1).Min(p => p.Start);
    }

    protected override Garden Parse(string input)
    {
        var seeds = new List<Seed>();
        var mapList = new List<List<Map>>();
        var maps = new List<Map>();

        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            if (line.StartsWith("seeds:"))
            {
                var arr = line.Split(':')[1].Trim().Split(' ');
                if (IsPart2) {
                    // for part 2, seeds are pairs (not individual ids), with the first being the start and the second the range.
                    for (var i=0; i<arr.Length; i+=2)
                    {
                        seeds.Add(new Seed() { Id = long.Parse(arr[i]), Range = long.Parse(arr[i+1]) });
                    }
                }
                else {
                    seeds.AddRange(arr.Select(p => new Seed() { Id = long.Parse(p) }));
                }
            }
            else
            {
                if (line.Contains("map"))
                {
                    if (maps.Any())
                    {
                        mapList.Add(maps);
                        maps = new List<Map>();     // maps.Clear not sufficient!  references!!
                    }
                }
                else
                {
                    var parts = line.Split(" ");
                    maps.Add(new Map() { Dest = long.Parse(parts[0]), Src = long.Parse(parts[1]), Range = long.Parse(parts[2]) });
                }
            }
        }

        // add the last map
        if (maps.Any())
        {
            mapList.Add(maps);
        }

        return new Garden() { Seeds = seeds, Maps = mapList };
    }
}