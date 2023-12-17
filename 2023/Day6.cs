namespace AOC.AOC2023;

public class Day6 : Day<List<Day6.Race>>
{
    protected override string? SampleRawInput { get => "Time:      7  15   30\nDistance:  9  40  200"; }

    protected override bool Part2ParsedDifferently => true;         // for part 2, all of the numbers are treated as a single number.

    public class Race
    {
        public long Time { get; set; }
        public long Distance { get; set; }
    }

    protected override long Part1()
    {
        return RunRaces();
    }

    protected override long Part2()
    {
        return RunRaces();
    }

    private long RunRaces()
    {
        var totalMargin = 1L;

        Input.ForEach(p => totalMargin *= GetMargin(p));

        return totalMargin;
    }

    private static long GetMargin(Race race)
    {
        // this can be done by brute force easily even with the real inputs (~300ms for part 2), but this is a quadratic with variable hold time and can be solved exactly.

        /* brute force
            var margin = 0;
            for (var i=1; i<race.Time; i++)
            {
                var dist = (race.Time - i) * i;
                if (dist > race.Distance)
                    margin++;
            }
        */

        // by formula
        // y = -x^2 + bx
        // y = -(x^2 - bx + b^2/4) + b^2/4
        // y = -(x - b/2)^2 + b^2/4
        // y + b^2/4 = -(x - b/2)^2
        // -(y + b^2/4) = (x - b/2)^2
        // x = b/2 +/- sqrt(-(y + b^2/4))

        // y is our minimum distance, b is the total race time, x is the hold time
        // if the quadratic has 0/1 real roots, then the minimum distance is never exceeded
        // if the quadratic has two real roots, then the minimum distance is exceeded for every integer between the roots.  note that a root that is an exact integer needs to be excluded!  

        var margin = 0;
        var det = -(race.Distance - race.Time*race.Time/4.0);
        if (det > 0)
        {
            var low = race.Time/2.0 - Math.Sqrt(det);
            var high = race.Time/2.0 + Math.Sqrt(det);
            margin = (int)(Math.Floor(high) - Math.Ceiling(low) + 1
                // exclude exact integer root(s)
                - (low == Math.Floor(low) ? 1 : 0) - (high == Math.Floor(high) ? 1 : 0));
        }

        return margin;
    }

    protected override List<Race> Parse(string input)
    {
        var races = new List<Race>();

        var times = new List<long>();
        var distances = new List<long>();

        foreach (var line in input.Split('\n'))
        {
             var parsed = Spaces().Replace(line, " ").Split(" ");
             if (parsed[0] == "Time:")
             {
                if (IsPart2)
                    times.Add(long.Parse(parsed.Skip(1).Aggregate((c, n) => c + n)));       // for part 2, all of the numbers are treated as a single number.
                else
                    times.AddRange(parsed.Skip(1).Select(long.Parse));
             }
             else if (parsed[0] == "Distance:")
             {
                if (IsPart2)
                    distances.Add(long.Parse(parsed.Skip(1).Aggregate((c, n) => c + n)));
                else
                    distances.AddRange(parsed.Skip(1).Select(long.Parse));
             }
        }

        for (var i=0; i<times.Count; i++)
        {
            races.Add(new Race() { Time = times[i], Distance = distances[i] });
        }

        return races;
    }
}