namespace AOC.AOC2023;

public class Day6 : Day<Day6.RaceGroup>
{
    protected override string? SampleRawInput { get => "Time:      7  15   30\nDistance:  9  40  200"; }

    public class RaceGroup
    {
        public required List<Race> Races { get; set; }

        public long TotalMargin()
        {
            var totalMargin = 1L;
            Races.ForEach(p => totalMargin *= p.GetMargin());
            return totalMargin;
        }
    }

    public class Race
    {
        public long Time { get; set; }
        public long Distance { get; set; }

        public long GetMargin()
        {
            // this can be done by brute force easily even with the real inputs (~300ms for part 2), but this is a quadratic with variable hold time and can be solved exactly.

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
            var det = -(Distance - Time*Time/4.0);
            if (det > 0)
            {
                var low = Time/2.0 - Math.Sqrt(det);
                var high = Time/2.0 + Math.Sqrt(det);
                margin = (int)(Math.Floor(high) - Math.Ceiling(low) + 1
                    // exclude exact integer root(s)
                    - (low == Math.Floor(low) ? 1 : 0) - (high == Math.Floor(high) ? 1 : 0));
            }

            return margin;
        }
    }

    protected override long Part1()
    {
        return Input.TotalMargin();
    }

    protected override long Part2()
    {
        return Input.TotalMargin();
    }
    
    protected override RaceGroup Parse(string input)
    {
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

        return new RaceGroup() { Races = times.Select((p, i) => new Race() { Time = p, Distance = distances[i] }).ToList() };
    }
}