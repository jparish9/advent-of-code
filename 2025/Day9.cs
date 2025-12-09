using System.Formats.Asn1;
using AOC.Utils;

namespace AOC.AOC2025;

public class Day9 : Day<Day9.Theater>
{
    protected override string? SampleRawInput { get => "7,1\n11,1\n11,7\n9,7\n9,5\n2,5\n2,3\n7,3"; }
    //protected override string? SampleRawInput { get => "1,1\n1,15\n15,15\n15,13\n3,13\n3,3\n15,3\n15,1"; }          // test c-shape concave polygon

    public class Theater
    {
        public required List<(long x, long y)> RedTiles { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.RedTiles.Select(p => Input.RedTiles.Select(q => Math.Abs(p.x - q.x + 1) * Math.Abs(p.y - q.y + 1)).Max()).Max();
    }

    protected override Answer Part2()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var maxArea = 0L;

        // compress the coordinates to a much smaller space (eliminating irrelevant empty space) and then brute-force checking within that much smaller space.
        // no need for polygon collision, line segment intersection, complex edge/collinearity handling, etc.
        // ~2-3 sec for the full input.
        var xMap = Input.RedTiles.Select(p => p.x).Distinct().OrderBy(x => x).ToList();
        var yMap = Input.RedTiles.Select(p => p.y).Distinct().OrderBy(y => y).ToList();

        var compressedRedTiles = Input.RedTiles.Select(p => (x: xMap.IndexOf(p.x), y: yMap.IndexOf(p.y))).ToList();

        var redOrGreen = new HashSet<(long x, long y)>();

        // iterate over the whole bounding box of the red tiles.
        var minX = compressedRedTiles.Min(p => p.x);
        var maxX = compressedRedTiles.Max(p => p.x);
        var minY = compressedRedTiles.Min(p => p.y);
        var maxY = compressedRedTiles.Max(p => p.y);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if (Polygon.IsPointInPolygon((x, y), compressedRedTiles))
                {
                    redOrGreen.Add((x, y));
                }
            }
        }

        // now check every possible rectangle defined by pairs of red tiles, but find the largest area using the original coordinates
        for (var i = 0; i < compressedRedTiles.Count; i++)
        {
            var (px, py) = compressedRedTiles[i];
            for (var j = i + 1; j < compressedRedTiles.Count; j++)
            {
                var (qx, qy) = compressedRedTiles[j];

                var minRectX = Math.Min(px, qx);
                var maxRectX = Math.Max(px, qx);
                var minRectY = Math.Min(py, qy);
                var maxRectY = Math.Max(py, qy);

                var area = (Math.Abs(xMap[px] - xMap[qx]) + 1) * (Math.Abs(yMap[py] - yMap[qy]) + 1);
                if (area <= maxArea) continue;          // no need to check, this rectangle is smaller than current max

                var allInside = true;

                for (var x = minRectX; x <= maxRectX; x++)
                {
                    for (var y = minRectY; y <= maxRectY; y++)
                    {
                        if (!redOrGreen.Contains((x, y)))
                        {
                            allInside = false;
                            break;
                        }
                    }
                    if (!allInside) break;
                }

                if (!allInside) continue;

                maxArea = area;
            }
        }

        return maxArea;
    }

    protected override Theater Parse(RawInput input)
    {
        return new Theater
        {
            RedTiles = [.. input.Lines().Select(line =>
            {
                var parts = line.Split(',');
                return (long.Parse(parts[0]), long.Parse(parts[1]));
            })]
        };
    }
}