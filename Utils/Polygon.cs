namespace AOC.Utils;

public class Polygon
{
    // Pick's theorem.  expects corners, with an override of the total number of borderLatticePoints to be used for b in Pick's theorem.
    // this could be improved slightly by counting all the lattice points that lie on the lines defenide by the corners, removing the need for the optional borderLatticePoints override.
    public static long CountInteriorLatticePoints(List<(long, long)> corners, long? borderLatticePoints = null)
    {
        var detSum = 0L;
        for (var i=0; i<corners.Count; i++)
        {
            detSum += corners[i].Item1 * corners[(i+1)%corners.Count].Item2 - corners[i].Item2 * corners[(i+1)%corners.Count].Item1;
        }

        detSum = Math.Abs(detSum);      // will be negative if the path is clockwise, positive if counter-clockwise

        var area = detSum/2;

        // using Pick's theorem, area = i + b/2 - 1 where b is the number of boundary points, i is the number of interior points (which we want)
        // i = area - b/2 + 1
        return area - (borderLatticePoints ?? corners.Count)/2 + 1;
    }
}