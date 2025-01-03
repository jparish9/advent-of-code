namespace AOC.AOC2023;

public class Day1 : Day<List<string>>
{
    protected override string? SampleRawInput { get => "1abc2\npqr3stu8vwx\na1b2c3d4e5f\ntreb7uchet"; }
    protected override string? SampleRawInputPart2 { get => "two1nine\neightwothree\nabcone2threexyz\nxtwone3four\n4nineeightseven2\nzoneight234\n7pqrstsixteen"; }

    protected override Answer Part1()
    {
        var match = new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9'};
        var sum = 0;
        foreach (var line in Input)
        {
            sum += int.Parse(string.Concat(line.AsSpan(line.IndexOfAny(match, 0), 1), line.AsSpan(line.LastIndexOfAny(match, line.Length-1), 1)));
        }
        return sum;
    }

    protected override Answer Part2()
    {
        var matches = new [] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var sum = 0;
        foreach (var line in Input)
        {
            var first = matches.Select(p => (match: p, pos: line.IndexOf(p))).Where(p => p.pos != -1).OrderBy(p => p.pos).Select(p => p.match).First();
            var last = matches.Select(p => (match: p, pos: line.LastIndexOf(p))).Where(p => p.pos != -1).OrderByDescending(p => p.pos).Select(p => p.match).First();

            if (int.TryParse(first, out var firstInt)) {
                sum += 10*firstInt;
            } else {
                sum += 10*(Array.IndexOf(matches, first)-8);
            }

            if (int.TryParse(last, out var lastInt)) {
                sum += lastInt;
            } else {
                sum += Array.IndexOf(matches, last)-8;
            }
        }

        return sum;
    }

    protected override List<string> Parse(RawInput input)
    {
        return input.Lines().ToList();
    }
}