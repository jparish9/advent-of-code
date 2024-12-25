namespace AOC.AOC2015;

public class Day14 : Day<Day14.ReindeerRace>
{
    protected override string? SampleRawInput { get => "Comet can fly 14 km/s for 10 seconds, but then must rest for 127 seconds.\n"
        + "Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds."; }

    public class ReindeerRace
    {
        public required List<Reindeer> Deer { get; set; }
    }

    public class Reindeer
    {
        public required string Name { get; set; }
        public int Speed { get; set; }
        public int FlyTime { get; set; }
        public int RestTime { get; set; }

        public int DistanceTravelled(int time)
        {
            var cycleTime = FlyTime + RestTime;
            var cycles = time / cycleTime;
            var remainder = time % cycleTime;

            return cycles * FlyTime * Speed + Math.Min(remainder, FlyTime) * Speed;
        }
    }

    protected override Answer Part1()
    {
        return Input.Deer.Max(p => p.DistanceTravelled(Input.Deer.Count == 2 ? 1000 : 2503));
    }

    protected override Answer Part2()
    {
        var scores = new Dictionary<string, int>();
        for (int i = 1; i <= (Input.Deer.Count == 2 ? 1000 : 2503); i++)
        {
            var leader = Input.Deer.OrderByDescending(p => p.DistanceTravelled(i)).First().Name;
            if (!scores.ContainsKey(leader))
            {
                scores.Add(leader, 0);
            }
            scores[leader]++;
        }

        return scores.Max(p => p.Value);
    }

    protected override ReindeerRace Parse(RawInput input)
    {
        var deer = new List<Reindeer>();
        foreach (var line in input.Lines())
        {
            var parts = line.Split(' ');
            var name = parts[0];
            var speed = int.Parse(parts[3]);
            var flyTime = int.Parse(parts[6]);
            var restTime = int.Parse(parts[13]);

            deer.Add(new Reindeer() { Name = name, Speed = speed, FlyTime = flyTime, RestTime = restTime });
        }

        return new ReindeerRace() { Deer = deer };
    }
}