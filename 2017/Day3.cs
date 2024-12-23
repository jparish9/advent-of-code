namespace AOC.AOC2017;

public class Day3 : Day<Day3.MemoryGrid>
{
    protected override string? SampleRawInput { get => "23"; }

    private static readonly (int X, int Y)[] Directions = [(1, 0), (0, -1), (-1, 0), (0, 1)];           // right, up, left, down

    public class MemoryGrid
    {
        public required int Size;

        private long[][] Grid = [];

        public (int X, int Y) LastPosition;

        public long Allocate(bool until)
        {
            var wl = (int)Math.Ceiling(Math.Sqrt(Size)) + 3;

            Grid = new long[wl][];
            for (var i = 0; i < wl; i++)
            {
                Grid[i] = new long[wl];
            }

            var startPosition = (X: wl / 2, Y: wl / 2);
            var position = startPosition;

            var val = 1;
            var step = 1;
            var directionNdx = 0;
            var direction = Directions[directionNdx];

            var stop = false;
            var sum = 0L;

            while (true)
            {
                for (var i=0; i<step; i++)
                {
                    sum = SumNeighbors(position);
                    Grid[position.Y][position.X] = sum;
                    if (!until && ++val > Size) { stop = true; break; }
                    if (until && sum > Size) { stop = true; break; }

                    position.X += direction.X;
                    position.Y += direction.Y;
                }

                if (stop) break;

                directionNdx = (directionNdx + 1) % 4;
                direction = Directions[directionNdx];
                if (directionNdx % 2 == 0) step++;
            }

            if (until) return sum;
            else return Math.Abs(position.X - startPosition.X) + Math.Abs(position.Y - startPosition.Y);
        }

        private long SumNeighbors((int X, int Y) position)
        {
            var sum = 0L;
            for (var y = position.Y - 1; y <= position.Y + 1; y++)
            {
                for (var x = position.X - 1; x <= position.X + 1; x++)
                {
                    sum += Grid[y][x];
                }
            }

            return sum == 0 ? 1 : sum;
        }
    }

    protected override Answer Part1()
    {
        return Input.Allocate(false);
    }

    protected override Answer Part2()
    {
        return Input.Allocate(true);
    }

    protected override MemoryGrid Parse(string input)
    {
        return new MemoryGrid() { Size = int.Parse(input) };
    }
}