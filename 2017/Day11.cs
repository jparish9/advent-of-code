namespace AOC.AOC2017;

public class Day11 : Day<Day11.HexPath>
{
    protected override string? SampleRawInput { get => "se,ne,se,ne,n"; }

    public class HexPath
    {
        public required List<string> PathTaken { get; set; }

        public Dictionary<int, Cache> Cache { get; set; } = [];
    }

    public class Cache
    {
        public int FinalDistance { get; set; }
        public int MaxDistance { get; set; }
    }

    // 488 too low?
    protected override Answer Part1()
    {
        WalkPath();
        return Input.Cache[InputHashCode].FinalDistance;
    }

    protected override Answer Part2()
    {
        WalkPath();
        return Input.Cache[InputHashCode].MaxDistance;
    }

    private void WalkPath()
    {
        if (Input.Cache.ContainsKey(InputHashCode)) return;         // already run for this input
        
        var x = 0;
        var y = 0;

        var maxDistance = 0;

        // we can turn this into a coordinate system where every connected hex is two manhattan steps in the standard x-y plane apart.
        // HOWEVER, for computing the shortest HEX path, it is impossible to go two steps in the x-direction in one "step", so the shortest path isn't quite just Manhattan distance.
        // (every +1 or -1 in the x-direction is always a full step, since moves of (2,0) and (-2,0) are not possible in the transformed coordinate system.)

        foreach (var step in Input.PathTaken)
        {
            if (step == "n")       { y += 2; }
            else if (step == "ne") { x += 1; y += 1; }
            else if (step == "se") { x += 1; y -= 1; }
            else if (step == "s")  { y -= 2; }
            else if (step == "sw") { x -= 1; y -= 1; }
            else if (step == "nw") { x -= 1; y += 1; }
            else { throw new Exception($"Invalid step '{step}'"); }

            var dist = HexDistance(x, y);
            if (dist > maxDistance) maxDistance = dist;
        }

        Input.Cache[InputHashCode] = new Cache { FinalDistance = HexDistance(x, y), MaxDistance = maxDistance };
    }

    // hex grid distance from origin, with coordinate system described above
    private static int HexDistance(int x, int y)
    {
        var absX = Math.Abs(x);
        var absY = Math.Abs(y);
        
        return (absX + absY + (absX > absY ? (absX - absY) : 0)) / 2;
    }

    protected override HexPath Parse(RawInput input)
    {
        return new HexPath
        {
            PathTaken = [.. input.Lines()[0].Split(',')]
        };
    }
}