namespace AOC.AOC2015;

public class Day3 : Day<Day3.Delivery>
{
    protected override string? SampleRawInput { get => "^>v<"; }
    public class Delivery
    {
        public required string Directions { get; set; }

        public HashSet<(int X, int Y)> Visited = new();
        public HashSet<(int X, int Y)> RoboVisited = new();

        public void Clear()
        {
            Visited.Clear();
            RoboVisited.Clear();
        }

        public static (int X, int Y) GetNextPosition((int X, int Y) current, char direction) =>
            direction switch
            {
                '^' => (current.X, current.Y + 1),
                'v' => (current.X, current.Y - 1),
                '>' => (current.X + 1, current.Y),
                '<' => (current.X - 1, current.Y),
                _ => throw new Exception($"Invalid direction {direction}")
            };
    }

    protected override Answer Part1()
    {
        Input.Clear();

        var pos = (X: 0, Y: 0);
        Input.Visited.Add(pos);

        for (var i=0; i<Input.Directions.Length; i++)
        {
            pos = Delivery.GetNextPosition(pos, Input.Directions[i]);
            Input.Visited.Add(pos);
        }

        return Input.Visited.Count;
    }

    protected override Answer Part2()
    {
        Input.Clear();

        var santaPos = (X: 0, Y: 0);
        var roboPos = (X: 0, Y: 0);
        Input.Visited.Add(santaPos);

        for (var i=0; i<Input.Directions.Length; i++)
        {
            if (i%2 == 0)
            {
                santaPos = Delivery.GetNextPosition(santaPos, Input.Directions[i]);
                Input.Visited.Add(santaPos);
            }
            else
            {
                roboPos = Delivery.GetNextPosition(roboPos, Input.Directions[i]);
                Input.RoboVisited.Add(roboPos);
            }
        }

        return Input.Visited.Union(Input.RoboVisited).Count();
    }

    protected override Delivery Parse(string input)
    {
        return new Delivery() { Directions = input };
    }
}