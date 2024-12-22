namespace AOC.AOC2024;

public class Day14 : Day<Day14.Swarm>
{
    protected override string? SampleRawInput { get => "p=0,4 v=3,-3\np=6,3 v=-1,-3\np=10,3 v=-1,2\np=2,0 v=2,-1\np=0,0 v=1,3\np=3,0 v=-2,-2\np=7,6 v=-1,-3\np=3,0 v=-1,-2\np=9,3 v=2,3\np=7,3 v=-1,2\np=2,4 v=2,-3\np=9,5 v=-3,-3"; }

    public class Swarm
    {
        public required List<Robot> Robots;
        public required int Width;
        public required int Height;

        public void Reset()
        {
            foreach (var robot in Robots)
            {
                robot.Position = robot.StartingPosition;
            }
        }

        public void Move(int seconds)
        {
            foreach (var robot in Robots)
            {
                var pos = (X: (robot.Position.X + robot.Velocity.X * seconds) % Width, Y: (robot.Position.Y + robot.Velocity.Y * seconds) % Height);
                if (pos.X < 0) pos.X += Width;          // % allows negative values
                if (pos.Y < 0) pos.Y += Height;

                robot.Position = pos;
            }
        }

        public void Print()
        {
            var totalCt = 0;
            for (var y=0; y<Height; y++)
            {
                for (var x=0; x<Width; x++)
                {
                    var ct = Robots.Count(r => r.Position.X == x && r.Position.Y == y);
                    Console.Write(ct > 9 ? "X" : (ct > 0 ? ct.ToString() : "."));
                    totalCt += ct;
                }
                Console.WriteLine();
            }
        }

        public long SafetyFactor()
        {
            return Robots.Count(p => p.Position.X < Width/2 && p.Position.Y < Height/2)
                * Robots.Count(p => p.Position.X < Width/2 && p.Position.Y > Height/2)
                * Robots.Count(p => p.Position.X > Width/2 && p.Position.Y < Height/2)
                * Robots.Count(p => p.Position.X > Width/2 && p.Position.Y > Height/2);
        }

        public long Stacked()
        {
            return Robots.Count - Robots.Select(p => p.Position).Distinct().Count();
        }
    }

    public class Robot
    {
        public required (int X, int Y) StartingPosition;
        public required (int X, int Y) Velocity;
        public (int X, int Y) Position;
    }

    protected override Answer Part1()
    {
        Input.Reset();
        Input.Move(100);

        return Input.SafetyFactor();
    }

    protected override Answer Part2()
    {
        if (Input.Robots.Count <= 12) return "ignored";         // ignore for sample

        Input.Reset();

        // there are a number of ways to determine which iteration (probably) displays an image, and a clue was given in part 1.  the following all work, at least with my input:
        // - minimum sum of variances of X and Y coordinates
        // - most number of robots with immediate neighbors
        // - minimum safety factor (most robots likely to be in one quadrant ==> lower safety factor)
        // - first iteration having all robots with unique positions

        // the only one of these that is deterministic is the last, but even that one doesn't mean there MUST be an image.  it's just a logical place to stop.
        var seconds = 0;
        var metric = Input.Stacked();
        while (metric > 0)
        {
            Input.Move(1);
            seconds++;
            metric = Input.Stacked();
        }

        return seconds;
    }

    protected override Swarm Parse(string input)
    {
        var robots = new List<Robot>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(" ");
            var pos = parts[0].Split("=")[1].Split(",");
            var vel = parts[1].Split("=")[1].Split(",");
            robots.Add(new Robot() { StartingPosition = (int.Parse(pos[0]), int.Parse(pos[1])), Velocity = (int.Parse(vel[0]), int.Parse(vel[1])) });
        }

        return new Swarm() { Robots = robots, Width = IsSampleInput ? 11 : 101, Height = IsSampleInput ? 7 : 103 };
    }
}