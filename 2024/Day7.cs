namespace AOC.AOC2024;

public class Day7 : Day<Day7.Bridge>
{
    protected override string? SampleRawInput { get => "190: 10 19\n3267: 81 40 27\n83: 17 5\n156: 15 6\n7290: 6 8 6 15\n161011: 16 10 13\n192: 17 8 14\n21037: 9 7 18 13\n292: 11 6 16 20"; }

    public class Bridge
    {
        public required List<Calibration> Calibrations;
    }
    
    public class Calibration
    {
        public required long TestValue;
        public required List<long> Operands;
    }

    private static readonly List<Func<long, long, long>> Operators = new()
    {
        (a, b) => a + b,
        (a, b) => a * b,
        (a, b) => long.Parse(a.ToString() + b.ToString())
    };

    protected override long Part1()
    {
        return Check(Operators.Take(2).ToList());
    }

    protected override long Part2()
    {
        return Check(Operators);
    }

    private long Check(List<Func<long, long, long>> operators)
    {
        var valid = 0L;
        foreach (var calibration in Input.Calibrations)
        {
            var numOperations = calibration.Operands.Count-1;
            // permute possible operations
            for (var i=0; i<Math.Pow(operators.Count, numOperations); i++)
            {
                var test = calibration.Operands[0];
                for (var j=0; j<numOperations; j++)
                {
                    var op = operators[(int)(i / Math.Pow(operators.Count, j) % operators.Count)];
                    test = op(test, calibration.Operands[j+1]);
                }
                if (test == calibration.TestValue)
                {
                    valid += calibration.TestValue;
                    break;
                }
            }
        }
        return valid;
    }

    protected override Bridge Parse(string input)
    {
        var bridge = new Bridge
        {
            Calibrations = new()
        };
        foreach (var line in input.Split("\n").Where(x => !string.IsNullOrEmpty(x)))
        {
            var parts = line.Split(": ");
            var calibration = new Calibration
            {
                TestValue = long.Parse(parts[0]),
                Operands = parts[1].Split(" ").Select(long.Parse).ToList()
            };
            bridge.Calibrations.Add(calibration);
        }
        return bridge;
    }
}