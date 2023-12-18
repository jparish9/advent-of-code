namespace AOC.Utils;

public class Polygon
{
    // Get the number of lattice points in or in and on a polygon, given an ordered list of its bounding lattice points.
    // Use the shoelace formula for the area of a polygon, then Pick's theorem to get the number of interior points.
    // Successive boundary points must be connectable with horizontal or vertical lines.
    public static long GridLatticePoints(List<(long, long)> orderedBoundary, bool interiorOnly)
    {
        var detSum = 0L;
        var boundaryPoints = 0L;
        for (var i=0; i<orderedBoundary.Count; i++)
        {
            detSum += orderedBoundary[i].Item1 * orderedBoundary[(i+1)%orderedBoundary.Count].Item2 - orderedBoundary[i].Item2 * orderedBoundary[(i+1)%orderedBoundary.Count].Item1;
            boundaryPoints += Math.Abs(orderedBoundary[i].Item1 - orderedBoundary[(i+1)%orderedBoundary.Count].Item1) + Math.Abs(orderedBoundary[i].Item2 - orderedBoundary[(i+1)%orderedBoundary.Count].Item2);
        }

        detSum = Math.Abs(detSum);      // will be negative if the path is clockwise, positive if counter-clockwise

        var area = detSum/2;

        // Pick's theorem: area = i + b/2 - 1 where b is the number of boundary points, i is the number of interior points (which we want)
        // i = area - b/2 + 1
        return area - boundaryPoints/2 + 1
            + (interiorOnly ? 0 : boundaryPoints);
    }
}