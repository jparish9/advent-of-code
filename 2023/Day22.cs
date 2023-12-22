namespace AOC.AOC2023;

public class Day22 : Day<Day22.BrickSnapshot>
{
    protected override string? SampleRawInput { get => "1,0,1~1,2,1\n0,0,2~2,0,2\n0,2,3~2,2,3\n0,0,4~0,2,4\n2,0,5~2,2,5\n0,1,6~2,1,6\n1,1,8~1,1,9"; }

    public class Brick
    {
        public (int X, int Y, int Z) From { get; set; }
        public (int X, int Y, int Z) To { get; set; }
    }

    public class BrickSnapshot
    {
        public required List<Brick> Bricks { get; set; }
        public Dictionary<int, HashSet<int>>? Supports { get; set; }         // to be calculated upon Settle()ing
        private bool _isSettled = false;

        // print profile maps as shown in the problem description
        public void PrintXZMap()
        {
            var minX = Bricks.Min(p => p.From.X);
            var maxX = Bricks.Max(p => p.To.X);
            var minZ = Bricks.Min(p => p.From.Z);
            var maxZ = Bricks.Max(p => p.To.Z);

            Console.WriteLine("X".PadLeft((maxX - minX)/2+1, ' '));
            if (maxX < 10)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    Console.Write(x);
                }
                Console.WriteLine();
            }

            for (var z = maxZ; z >= 0; z--)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (z == 0) Console.Write("-");
                    else
                    {
                        var bricks = Bricks.Where(p => p.From.X <= x && p.To.X >= x && p.From.Z <= z && p.To.Z >= z);
                        Console.Write(!bricks.Any() ? "." : (bricks.Count() > 1 ? "?" : (char)('A' + Bricks.IndexOf(bricks.First()))));
                    }
                }
                Console.Write(" " + z);
                if (z == maxZ/2 + 1) Console.Write(" Z");
                Console.WriteLine();
            }
        }

        public void PrintYZMap()
        {
            var minY = Bricks.Min(p => p.From.Y);
            var maxY = Bricks.Max(p => p.To.Y);
            var minZ = Bricks.Min(p => p.From.Z);
            var maxZ = Bricks.Max(p => p.To.Z);

            Console.WriteLine("Y".PadLeft((maxY - minY)/2+1, ' '));
            if (maxY < 10)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    Console.Write(y);
                }
                Console.WriteLine();
            }

            for (var z = maxZ; z >= 0; z--)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    if (z == 0) Console.Write("-");
                    else
                    {
                        var bricks = Bricks.Where(p => p.From.Y <= y && p.To.Y >= y && p.From.Z <= z && p.To.Z >= z);
                        Console.Write(!bricks.Any() ? "." : (bricks.Count() > 1 ? "?" : (char)('A' + Bricks.IndexOf(bricks.First()))));
                    }
                }
                Console.Write(" " + z);
                if (z == maxZ/2 + 1) Console.Write(" Z");
                Console.WriteLine();
            }
        }

        public void Settle()
        {
            if (_isSettled) return;

            var settled = false;
            while (!settled)
            {
                settled = true;
                var i=0;
                foreach (var brick in Bricks)
                {
                    if (brick.From.Z == 1) continue;            // this brick is on the ground already

                    // check for no bricks below this one, if none found, move this brick down one Z level, flag that there was something to do, and continue.
                    if (!Bricks.Any(p => p.To.Z == brick.From.Z - 1 &&
                        ((p.From.X <= brick.From.X && p.To.X >= brick.From.X) || (p.From.X >= brick.From.X && p.From.X <= brick.To.X))
                        && ((p.From.Y <= brick.From.Y && p.To.Y >= brick.From.Y) || (p.From.Y >= brick.From.Y && p.From.Y <= brick.To.Y))))
                    {
                        brick.From = (brick.From.X, brick.From.Y, brick.From.Z - 1);
                        brick.To = (brick.To.X, brick.To.Y, brick.To.Z - 1);
                        settled = false;
                    }
                    i++;
                }
            }

            CalculateSupports();

            _isSettled = true;
        }

        private void CalculateSupports()
        {
            Supports = new Dictionary<int, HashSet<int>>();

            for (var i=0; i<Bricks.Count; i++)
            {
                Supports[i] = new HashSet<int>();
                for (var j=0; j<Bricks.Count; j++)
                {
                    if (i == j) continue;

                    if (Bricks[j].From.Z == Bricks[i].To.Z + 1 &&
                        ((Bricks[j].From.X <= Bricks[i].From.X && Bricks[j].To.X >= Bricks[i].From.X) || (Bricks[j].From.X >= Bricks[i].From.X && Bricks[j].From.X <= Bricks[i].To.X))
                        && ((Bricks[j].From.Y <= Bricks[i].From.Y && Bricks[j].To.Y >= Bricks[i].From.Y) || (Bricks[j].From.Y >= Bricks[i].From.Y && Bricks[j].From.Y <= Bricks[i].To.Y)))
                    {
                        Supports[i].Add(j);         // brick i supports brick j
                    }
                }
            }
        }
    }

    protected override long Part1()
    {
        Input.Settle();         // sets Input.Supports
        var canBeRemoved = 0;
        foreach (var kv in Input.Supports!)
        {
            // a brick can be removed if it is not the only support of any other brick.
            if (!kv.Value.Any(v => Input.Supports.Where(p => p.Key != kv.Key).All(p => !p.Value.Contains(v))))
                canBeRemoved++;
        }

        return canBeRemoved;
    }

    protected override long Part2()
    {
        // this runs in about 45 seconds despite everything we are doing lookups on being a dictionary or a hashset,
        // and not doing the work of Settling or calculating the supports again.
        // there may be further ways to optimize but this is definitely good enough.

        Input.Settle();         // sets Input.Supports
        var ct = 0;
        foreach (var kv in Input.Supports!)
        {
            // count the number of bricks that would fall if this brick was removed, and follow the chain of support upward.
            var wouldFall = kv.Value.Where(v => Input.Supports.Where(p => p.Key != kv.Key).All(p => !p.Value.Contains(v))).ToList();
            var totalWouldFall = new HashSet<int>(wouldFall);
            WalkSupportChain(totalWouldFall, wouldFall);

            ct += totalWouldFall.Count;
        }

        return ct;
    }

    private void WalkSupportChain(HashSet<int> totalWouldFall, List<int> wouldFall)
    {
        foreach (var brick in wouldFall)
        {
            var newFall = Input.Supports![brick].Where(v => Input.Supports.Where(p => !totalWouldFall.Contains(p.Key)).All(p => !p.Value.Contains(v))).ToList();
            newFall.ForEach(p => totalWouldFall.Add(p));

            WalkSupportChain(totalWouldFall, newFall);
        }
    }

    protected override BrickSnapshot Parse(string input)
    {
        var bricks = new List<Brick>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split("~");
            var from = parts[0].Split(",").Select(int.Parse).ToArray();
            var to = parts[1].Split(",").Select(int.Parse).ToArray();

            bricks.Add(new Brick
            {
                From = (from[0], from[1], from[2]),
                To = (to[0], to[1], to[2])
            });
        }

        return new BrickSnapshot { Bricks = bricks };
    }
}