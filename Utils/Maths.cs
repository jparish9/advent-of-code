namespace AOC.Utils;

public class Maths
{
    public static long LeastCommonMultiple(long a, long b)
    {
        return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
    }

    public static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}