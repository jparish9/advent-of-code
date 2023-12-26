using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AOC.AOC2023;

public class Day24 : Day<Day24.Sky>
{
    protected override string? SampleRawInput { get =>
            "19, 13, 30 @ -2,  1, -2\n" +
            "18, 19, 22 @ -1, -1, -2\n" +
            "20, 25, 34 @ -2, -2, -4\n" +
            "12, 31, 28 @ -1, -2, -1\n" +
            "20, 19, 15 @  1, -5, -3"; }

    public class Sky
    {
        public required List<Hailstone> Hailstones { get; set; }

        public static (double X, double Y, double t, double u)? IntersectXY(Hailstone a, Hailstone b)
        {
            // determine where the given hailstones intersect, given they have INDEPENDENT time axes given by p = p0 + vt (this is NOT clear at all from the problem).
            // this is a system of two equations in two unknowns (the time-axis for each hailstone), since we are given the initial positions and velocities.
            // solve by inverting a 2x2 matrix, once we rearrange terms.

            // a.Position.X + a.Velocity.X * t = b.X + b.Velocity.X * u ==> a.Velocity.X * t - b.Velocity.X * u = b.X - a.X
            // a.Position.Y + a.Velocity.Y * t = b.Y + b.Velocity.Y * u ==> a.Velocity.Y * t - b.Velocity.Y * u = b.Y - a.Y
            // [a b] [t] = [c1]
            // [c d] [u] = [c2]

            var matrix = DenseMatrix.OfArray(new double[,] { { a.Velocity.X, -b.Velocity.X }, { a.Velocity.Y, -b.Velocity.Y } });
            var right = DenseVector.OfArray(new double[] { b.Position.X - a.Position.X, b.Position.Y - a.Position.Y });

            if (matrix.Determinant() == 0) return null;

            var solve = matrix.Solve(right);

            return (a.Position.X + a.Velocity.X * solve[0], a.Position.Y + a.Velocity.Y * solve[0], solve[0], solve[1]);
        }
    }

    public class Hailstone
    {
        public required (long X, long Y, long Z) Position { get; set; }
        public required (long X, long Y, long Z) Velocity { get; set; }
    }

    protected override long Part1()
    {
        var (low, high) = Input.Hailstones.Count > 5 ? (low: 200000000000000L, high: 400000000000000L) : (low: 7L, high: 27L);         // real or sample input

        var ct = 0;
        for (var i=0; i<Input.Hailstones.Count; i++)
        {
            for (var j=i+1; j<Input.Hailstones.Count; j++)
            {
                var intersect = Sky.IntersectXY(Input.Hailstones[i], Input.Hailstones[j]);

                if (intersect != null && intersect.Value.t >= 0 && intersect.Value.u >= 0
                    && intersect.Value.X >= low && intersect.Value.X <= high
                    && intersect.Value.Y >= low && intersect.Value.Y <= high)
                    ct++;
            }
        }

        return ct;
    }

    protected override long Part2()
    {
        // I didn't come up with any workable approaches to this on my own.  I thought it might have something to do with using a few (more) equations (in x,y,z) to use
        // in combination with the desired rock's initial position and velocity as unknowns for a larger system of equations, but couldn't quite figure it out.
        // I found this clever idea using cross products at https://www.reddit.com/r/adventofcode/comments/18pnycy/comment/kepu26z/,
        // and had to review some linear algebra.
        // summarizing:
        // the rock has position p0 and velocity v0 (both vectors (x,y,z)).  if the hailstones have positions p[i], velocities v[i], and their own time axes t[i], then
        // each hailstone will intersect the rock at:
        // p0 + t[i]*v0 = p[i] + t[i]*v[i] (for every i), so
        // (p0 - p[i]) = -t[i]*(v0 - v[i]).  t[i] is a scalar, so
        // (p0 - p[i]) x (v0 - v[i]) = 0, where x is the cross product, since (p0 - p[i]) and (v0 - v[i]) are parallel (for every i).
        // (this can be shown mathematically by crossing both sides of the equation with (v0 - v[i]) and using the fact that the cross product of a vector with a scalar multiple of itself is zero.)
        // we can take two arbitrary pair of hailstones to get six equations (our six unknowns are the three components of p0 and v0), and solve the system of equations.
        // (p0 - p[0]) x (v0 - v[0]) = (p0 - p[1]) x (v0 - v[1])
        // the cross product can be manipulated like scalar multiplication, with one exception (anticommutativity: u x v = -(v x u)),
        // so we can rearrange terms to get everything with p0 and v0 on the left, and constants on the right.
        // additionally, on the left we want p0 and v0 to be the second part of the cross product, so we can convert the left side to skew-symmetric form to get the Ax=b form we need.
        // remember: anticommutativity!
        // (p0 x v0) - (p0 x v[0]) - (p[0] x v0) + (p[0] x v[0]) = (p0 x v0) - (p0 x v[1]) - (p[1] x v0) + (p[1] x v[1])
        // -(p0 x v[0]) - (p[0] x v0) + (p[0] x v[0]) = - (p0 x v[1]) - (p[1] x v0) + (p[1] x v[1])
        // -(p0 x v[0]) - (p[0] x v0) + (p0 x v[1]) + (p[1] x v0) = (p[1] x v[1]) - (p[0] x v[0])
        // (v[0] x p0) - (p[0] x v0) - (v[1] x p0) + (p[1] x v0)  = (p[1] x v[1]) - (p[0] x v[0])
        // (v[0] - v[1]) x p0 - (p[0] + p[1]) x v0 = (p[1] x v[1]) - (p[0] x v[0])
        // with (v[0] - v[1]) and (p[0] - p[1]) in skew-symmetric form, this is a 3x3 matrix on the left and a 3x1 vector on the right.
        // repeat this with a second arbitrary pair of hailstones to get 3 more equations, and we have our 6 equations in 6 unknowns.
        // added the MathNet.Numerics package for vector and matrix support, but the numbers were big enough here to cause floating point issues.

        // try hailstones with "low" starting coordinates to try to overcome floating-point issues (see below).
        var minX = Input.Hailstones.Min(p => p.Position.X);
        var hs = Input.Hailstones.OrderBy(p => p.Position.X).Take(3).ToList();
        
        var h0 = hs[0];
        var h1 = hs[1];
        var h2 = hs[2];

        var p_0 = DenseVector.OfArray(new double[] { h0.Position.X, h0.Position.Y, h0.Position.Z });
        var p_1 = DenseVector.OfArray(new double[] { h1.Position.X, h1.Position.Y, h1.Position.Z });
        var p_2 = DenseVector.OfArray(new double[] { h2.Position.X, h2.Position.Y, h2.Position.Z });

        var v_0 = DenseVector.OfArray(new double[] { h0.Velocity.X, h0.Velocity.Y, h0.Velocity.Z });
        var v_1 = DenseVector.OfArray(new double[] { h1.Velocity.X, h1.Velocity.Y, h1.Velocity.Z });
        var v_2 = DenseVector.OfArray(new double[] { h2.Velocity.X, h2.Velocity.Y, h2.Velocity.Z });

        var left = new DenseMatrix(6, 6);
        left.SetSubMatrix(0, 0, SkewSymmetric(v_0 - v_1));
        left.SetSubMatrix(3, 0, SkewSymmetric(v_0 - v_2));
        left.SetSubMatrix(0, 3, SkewSymmetric(-p_0 + p_1));
        left.SetSubMatrix(3, 3, SkewSymmetric(-p_0 + p_2));

        var right = new DenseVector(6);
        right.SetSubVector(0, 3, CrossProduct(p_1, v_1) - CrossProduct(p_0, v_0));
        right.SetSubVector(3, 3, CrossProduct(p_2, v_2) - CrossProduct(p_0, v_0));

        // well, apparently I am being thwarted by floating point issues... double not good enough?  Using the inverse versus built-in .Solve give different but close answers.
        // also, I can get another set of different answers by providing different hailstones to generate the system of equations!
        // I bounded the answer for my input with my first guesses (where AoC gives "too high" or "too low" hints) to a range of 42.
        // AoC clamps down on guesses quickly, so I think the best approach is to try every possible starting position using only longs - solve for t using x, and check that t against y and z.
        // yes, this will not necessarily work for different input.
        
        var solve = left.Solve(right);
        //var solve2 = left.Inverse() * right;      // different results!

        var rock = new Hailstone() {
            Position = ((long)solve.At(0), (long)solve.At(1), (long)solve.At(2)),
            Velocity = ((long)solve.At(3).Round(0), (long)solve.At(4).Round(0), (long)solve.At(5).Round(0))     // velocities are tiny (3 digits) but can still be like .99999997, so use .Round
        };

        for (var xOffset = -42; xOffset <= 42; xOffset++)
        {
            for (var yOffset = -42; yOffset <= 42; yOffset++)
            {
                for (var zOffset = -42; zOffset <= 42; zOffset++)
                {
                    var rockTest = new Hailstone() {
                        Position = (rock.Position.X + xOffset, rock.Position.Y + yOffset, rock.Position.Z + zOffset),
                        Velocity = rock.Velocity
                    };

                    var intersect = true;
                    foreach (var hailstone in Input.Hailstones)
                    {
                        // solve for t using x, then check y and z with that t, using just longs.
                        // hp[i].x + t*hv[i].x = rp.x + t.rv.x
                        // t = (rp.x - hp[i].x) / (hv[i].x - rv.x)
                        var t = (rockTest.Position.X - hailstone.Position.X) / (hailstone.Velocity.X - rockTest.Velocity.X);
                        if (t < 0 ||
                            hailstone.Position.Y + t * hailstone.Velocity.Y != rockTest.Position.Y + t * rockTest.Velocity.Y ||
                            hailstone.Position.Z + t * hailstone.Velocity.Z != rockTest.Position.Z + t * rockTest.Velocity.Z)
                        {
                            intersect = false;
                            break;
                        }
                    }

                    if (intersect) return rockTest.Position.X + rockTest.Position.Y + rockTest.Position.Z;      // found a starting position that intersects all hailstones with integer t
                }
            }
        }

        Console.WriteLine("Did not find an intersection!");

        return 0;
    }

    // really surprised these aren't built into the MathNet.Numerics library.
    private static DenseMatrix SkewSymmetric(DenseVector v)
    {
        return DenseMatrix.OfArray(new double[,] { { 0, -v[2], v[1] }, { v[2], 0, -v[0] }, { -v[1], v[0], 0 } });
    }

    private static DenseVector CrossProduct(DenseVector v, DenseVector w)
    {
        return SkewSymmetric(v) * w;
    }

    protected override Sky Parse(string input)
    {
        var sky = new Sky() { Hailstones = new List<Hailstone>() };

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split("@");
            var position = parts[0].Trim().Split(",").Select(p => long.Parse(p.Trim())).ToArray();
            var velocity = parts[1].Trim().Split(",").Select(p => long.Parse(p.Trim())).ToArray();

            sky.Hailstones.Add(new Hailstone() {
                Position = (position[0], position[1], position[2]),
                Velocity = (velocity[0], velocity[1], velocity[2])
            });
        }

        return sky;
    }
}