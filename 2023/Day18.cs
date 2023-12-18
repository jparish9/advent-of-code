namespace AOC.AOC2023;

public class Day18 : Day<Day18.Digger>
{
    protected override string? SampleRawInput { get => "R 6 (#70c710)\nD 5 (#0dc571)\nL 2 (#5713f0)\nD 2 (#d2c081)\nR 2 (#59c680)\nD 2 (#411b91)\nL 5 (#8ceee2)\nU 2 (#caa173)\nL 1 (#1b58a2)\nU 2 (#caa171)\nR 2 (#7807d2)\nU 3 (#a77fa3)\nL 2 (#015232)\nU 2 (#7a21e3)"; }
    protected override bool Part2ParsedDifferently => true;

    public class Digger
    {
        public required List<Instruction> Instructions { get; set; }
        public (long, long) Position { get; set; } = (0, 0);
        public List<(long, long)> Corners { get; set; } = new List<(long, long)>();

        public void ApplyInstructions()
        {
            // reset
            Position = (0, 0);
            Corners = new List<(long, long)> { Position };

            // save corners
            var newPosition = Position;
            foreach (var instruction in Instructions)
            {
                newPosition.Item1 += instruction.Direction.Item1 * instruction.Distance;
                newPosition.Item2 += instruction.Direction.Item2 * instruction.Distance;

                if (!Corners.Contains(newPosition))
                    Corners.Add(newPosition);
            }
        }

        public long LagoonArea()
        {
            ApplyInstructions();

            var detSum = 0L;
            var borderPoints = 0L;
            for (var i=0; i<Corners.Count; i++)
            {
                detSum += Corners[i].Item1 * Corners[(i+1)%Corners.Count].Item2 - Corners[i].Item2 * Corners[(i+1)%Corners.Count].Item1;
                borderPoints += Math.Abs(Corners[i].Item1 - Corners[(i+1)%Corners.Count].Item1) + Math.Abs(Corners[i].Item2 - Corners[(i+1)%Corners.Count].Item2);
            }

            detSum = Math.Abs(detSum);      // will be negative if the path is clockwise, positive if counter-clockwise

            var area = detSum/2;

            // using Pick's theorem, area = i + b/2 - 1 where b is the number of boundary lattice points, i is the number of interior lattice points.
            // i = area - b/2 + 1
            return borderPoints
                + area - borderPoints/2 + 1;
        }
    }

    public class Instruction
    {
        public (int, int) Direction { get; set; }
        public long Distance { get; set; }
    }

    protected override long Part1()
    {
        return Input.LagoonArea();
    }

    protected override long Part2()
    {
        return Input.LagoonArea();
    }

    protected override Digger Parse(string input)
    {
        var instructions = new List<Instruction>();

        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            var parts = line.Split(' ');
            if (IsPart2)
            {
                instructions.Add(new Instruction()
                {
                    Direction = parts[2][7] switch {
                        '0' => (1,0),
                        '1' => (0,-1),
                        '2' => (-1,0),
                        '3' => (0,1),
                        _ => throw new Exception("bad direction")
                    },
                    Distance = int.Parse(parts[2][2..7], System.Globalization.NumberStyles.HexNumber)
                });
            }
            else
            {
                instructions.Add(new Instruction()
                {
                    Direction = parts[0][0] switch {
                        'R' => (1,0),
                        'D' => (0,-1),
                        'L' => (-1,0),
                        'U' => (0,1),
                        _ => throw new Exception("bad direction")
                    },
                    Distance = int.Parse(parts[1])
                });
            }
        }

        return new Digger() { Instructions = instructions };
    }
}