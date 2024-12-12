using AOC.Utils;

namespace AOC.AOC2023;

public class Day18 : Day<Day18.Digger>
{
    protected override string? SampleRawInput { get => "R 6 (#70c710)\nD 5 (#0dc571)\nL 2 (#5713f0)\nD 2 (#d2c081)\nR 2 (#59c680)\nD 2 (#411b91)\nL 5 (#8ceee2)\nU 2 (#caa173)\nL 1 (#1b58a2)\nU 2 (#caa171)\nR 2 (#7807d2)\nU 3 (#a77fa3)\nL 2 (#015232)\nU 2 (#7a21e3)"; }

    public class Digger
    {
        public required List<Instruction> Instructions { get; set; }
        public (long X, long Y) Position { get; set; } = (0, 0);
        public List<(long, long)> Corners { get; set; } = new List<(long, long)>();

        public void ApplyInstructions()
        {
            // reset
            Position = (0, 0);
            Corners = new List<(long X, long Y)> { Position };

            // save corners
            var newPosition = Position;
            foreach (var instruction in Instructions)
            {
                newPosition.X += instruction.Direction.X * instruction.Distance;
                newPosition.Y += instruction.Direction.Y * instruction.Distance;

                if (!Corners.Contains(newPosition))
                    Corners.Add(newPosition);
            }
        }

        public long LagoonArea()
        {
            ApplyInstructions();
            return Polygon.GridLatticePoints(Corners, false);
        }
    }

    public class Instruction
    {
        public (int X, int Y) Direction { get; set; }
        public long Distance { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.LagoonArea();
    }

    protected override Answer Part2()
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