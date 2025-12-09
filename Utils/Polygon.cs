namespace AOC.Utils;

public class Polygon
{
    // Get the number of lattice points in or in and on a polygon, given an ordered list of its bounding lattice points.
    // Use the shoelace formula for the area of a polygon, then Pick's theorem to get the number of interior points.
    // Successive boundary points must be connectable with horizontal or vertical lines.
    public static long GridLatticePoints(List<(long X, long Y)> orderedBoundary, bool interiorOnly)
    {
        var detSum = 0L;
        var boundaryPoints = 0L;
        for (var i=0; i<orderedBoundary.Count; i++)
        {
            detSum += orderedBoundary[i].X * orderedBoundary[(i+1)%orderedBoundary.Count].Y - orderedBoundary[i].Y * orderedBoundary[(i+1)%orderedBoundary.Count].X;
            boundaryPoints += Math.Abs(orderedBoundary[i].X - orderedBoundary[(i+1)%orderedBoundary.Count].X) + Math.Abs(orderedBoundary[i].Y - orderedBoundary[(i+1)%orderedBoundary.Count].Y);
        }

        detSum = Math.Abs(detSum);      // will be negative if the path is clockwise, positive if counter-clockwise

        var area = detSum/2;

        // Pick's theorem: area = i + b/2 - 1 where b is the number of boundary points, i is the number of interior points (which we want)
        // i = area - b/2 + 1
        return area - boundaryPoints/2 + 1
            + (interiorOnly ? 0 : boundaryPoints);
    }

    public static bool IsPointInPolygon((int X, int Y) point, List<(int X, int Y)> polygon, bool onEdgeIsInside = true)
    {
        var inside = false;
        var n = polygon.Count;
        for (int i=0, j=n-1; i<n; j=i++)
        {
            var xi = polygon[i].X;
            var yi = polygon[i].Y;
            var xj = polygon[j].X;
            var yj = polygon[j].Y;

            if (onEdgeIsInside)
            {
                // check edge case, literally
                var onEdge = (point.Y - yi) * (xj - xi) == (point.X - xi) * (yj - yi)
                    && Math.Min(xi, xj) <= point.X && point.X <= Math.Max(xi, xj)
                    && Math.Min(yi, yj) <= point.Y && point.Y <= Math.Max(yi, yj);
                if (onEdge) return true;
            }

            var intersect = ((yi > point.Y) != (yj > point.Y))
                && (point.X < (xj - xi) * (point.Y - yi) / (yj - yi + 0.0) + xi);
            if (intersect) inside = !inside;
        }

        return inside;
    }
}