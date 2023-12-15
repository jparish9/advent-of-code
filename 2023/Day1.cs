namespace AOC.AOC2023;

public class Day1 : Day<List<string>>
{
    protected override string? SampleRawInput { get => "1abc2\npqr3stu8vwx\na1b2c3d4e5f\ntreb7uchet"; }
    protected override string? SampleRawInputPart2 { get => "two1nine\neightwothree\nabcone2threexyz\nxtwone3four\n4nineeightseven2\nzoneight234\n7pqrstsixteen"; }

    protected override long Part1()
    {
        var match = new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9'};
        var sum = 0;
        foreach (var line in Input)
        {
            sum += int.Parse(string.Concat(line.AsSpan(line.IndexOfAny(match, 0), 1), line.AsSpan(line.LastIndexOfAny(match, line.Length-1), 1)));
        }
        return sum;
    }

    protected override long Part2()
    {
        var matches = new [] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var sum = 0;
        foreach (var line in Input)
        {
            var first = matches.Select(p => new Tuple<string, int>(p, line.IndexOf(p))).Where(p => p.Item2 != -1).OrderBy(p => p.Item2).First();
            var last = matches.Select(p => new Tuple<string, int>(p, line.LastIndexOf(p))).Where(p => p.Item2 != -1).OrderByDescending(p => p.Item2).First();

            if (int.TryParse(first.Item1, out var firstInt)) {
                sum += 10*firstInt;
            } else {
                sum += 10*(Array.IndexOf(matches, first.Item1)-8);
            }

            if (int.TryParse(last.Item1, out var lastInt)) {
                sum += lastInt;
            } else {
                sum += Array.IndexOf(matches, last.Item1)-8;
            }
        }

        return sum;
    }

    protected override List<string> Parse(string input)
    {
        return input.Split('\n').Where(p => p != "").ToList();
    }
}