using System.Runtime.CompilerServices;

namespace AOC.AOC2016;

public class Day1 : Day<Day1.Directions>
{
    protected override string? SampleRawInput { get => "R8, R4, R4, R8"; }

    public class Directions
    {
        public required List<string> Direction { get; set; }

        private (int X, int Y) _position;

        private HashSet<(int X, int Y)> _visited { get; set; } = new HashSet<(int X, int Y)>();

        public int Move(bool part2 = false)
        {
            _position = (0, 0);
            _visited = new HashSet<(int X, int Y)>();

            var dirs = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };     // north, east, south, west
            var dirNdx = 0;     // start facing north
            var vis = false;
            foreach (var dir in Direction)
            {
                var turn = dir[0];
                var distance = int.Parse(dir[1..]);

                if (turn == 'R') dirNdx = (dirNdx + 1) % 4;
                else if (turn == 'L') dirNdx = (dirNdx + 3) % 4;

                var currentDir = dirs[dirNdx];

                for (var i = 0; i < distance; i++)
                {
                    _position.X += currentDir.Item1;
                    _position.Y += currentDir.Item2;

                    if (part2 && _visited.Contains(_position))
                    {
                        vis = true;
                        break;
                    }
                    _visited.Add(_position);
                }
                if (vis) break;
            }

            return Math.Abs(_position.X) + Math.Abs(_position.Y);
        }
    }

    protected override long Part1()
    {
        return Input.Move();
    }

    protected override long Part2()
    {
        return Input.Move(true);
    }

    protected override Directions Parse(string input)
    {
        return new Directions() { Direction = input.Split(", ").ToList() };
    }
}