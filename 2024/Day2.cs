namespace AOC.AOC2024;

public class Day2 : Day<Day2.ReportSet>
{
    protected override string? SampleRawInput { get => "7 6 4 2 1\n1 2 7 8 9\n9 7 6 2 1\n1 3 2 4 5\n8 6 4 4 1\n1 3 6 7 9"; }

    public class ReportSet
    {
        public required List<Report> Reports { get; set; }
    }

    public class Report
    {
        public required List<int> Values { get; set; }
        public bool IsSafeWithoutDampener { get; set; }        // cache result from part 1; default unsafe
    }

    protected override Answer Part1()
    {
        var safeCount = 0;
        Input.Reports.ForEach(p => {
            p.IsSafeWithoutDampener = SafeCheck(p.Values);
            safeCount += p.IsSafeWithoutDampener ? 1 : 0;
        });
        return safeCount;
    }

    protected override Answer Part2()
    {
        var safeCount = Input.Reports.Count(p => p.IsSafeWithoutDampener!);
        foreach (var report in Input.Reports.Where(p => !p.IsSafeWithoutDampener!))
        {
            for (var i=0; i<report.Values.Count; i++)
            {
                if (SafeCheck(report.Values.Take(i).Concat(report.Values.Skip(i+1)).ToList()))          // make copy of list without i-th element and check again
                {
                    safeCount++;
                    break;
                }
            }
        }
        return safeCount;
    }

    private static bool SafeCheck(List<int> values)
    {
        var safe = true;
        var incType = values[1] > values[0] ? 1 : -1;
        for (var i=1; i<values.Count; i++)
        {
            // incrementing type changed, or equal values found
            if (values[i]*incType <= values[i-1]*incType)
            {
                safe = false;
                break;
            }

            // successive values differ by more than 3 in proper direction
            if (incType * (values[i] - values[i-1]) > 3)
            {
                safe = false;
                break;
            }
        }

        return safe;
    }

    protected override ReportSet Parse(RawInput input)
    {
        return new ReportSet() { Reports = input.Lines().Select(p => new Report() { Values = p.Split(" ").Select(int.Parse).ToList() }).ToList() };
    }
}