using System.Text.RegularExpressions;

namespace AOC.AOC2025;

public partial class Day2 : Day<Day2.Ranges>
{
    protected override string? SampleRawInput { get => "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124"; }

    public class Ranges
    {
        public required List<(long start, long end)> Range { get; set; }
    }

    protected override Answer Part1()
    {
        return GetInvalidSum(false);
    }

    protected override Answer Part2()
    {
        return GetInvalidSum(true);
    }

    private long GetInvalidSum(bool part2)
    {
        var invalidSum = 0L;
        foreach (var (start, end) in Input.Range)
        {
            for (long i=start; i <= end; i++)
            {
                if (CheckInvalid(i, part2)) invalidSum += i;
            }
        }

        return invalidSum;
    }

    private static bool CheckInvalid(long i, bool part2)
    {
        if (i < 10) return false;

        var str = i.ToString();
        if (!part2 && str.Length % 2 == 1) return false;            // ignore (e.g.) 999 for part 1

        var foundInvalid = false;

        for (var j=part2 ? 1 : str.Length / 2; j <= str.Length / 2; j++)            // check n-digit sections up to len/2; for part 1, only check len/2
        {
            if (str.Length % j != 0) continue;                             // total length not evenly divisible by this section length

            var invalid = true;
            var pattern = str[..j];

            var k = j;
            while (k < str.Length)
            {
                var nextPattern = str[k..(k + j)];
                if (nextPattern != pattern)
                {
                    invalid = false;
                    break;
                }
                k += j;
            }

            if (invalid)
            {
                foundInvalid = true;
                break;
            }
        }

        return foundInvalid;
    }

    // this works and is more "clever", but much slower than directly checking for repeating patterns of numbers
    // 5-10x slower even with compiled regex!
    /*
    private static bool CheckInvalidRegex(long i, bool part2)
    {
        return (part2 ? Part2Regex() : Part1Regex()).IsMatch(i.ToString());
    }
    */

    protected override Ranges Parse(RawInput input)
    {
        return new Ranges
        {
            Range = [.. input.Lines()[0].Split(',').Select(r =>
            {
                var parts = r.Split('-');
                return (long.Parse(parts[0]), long.Parse(parts[1]));
            })]
        };
    }

    [GeneratedRegex(@"^(\d+)\1$")]
    protected static partial Regex Part1Regex();

    [GeneratedRegex(@"^(\d+)\1{1,}$")]
    protected static partial Regex Part2Regex();
}