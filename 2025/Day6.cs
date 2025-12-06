namespace AOC.AOC2025;

public class Day6 : Day<Day6.CephalopodMath>
{
    protected override string? SampleRawInput { get => "123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +  "; }

    public class CephalopodMath
    {
        public List<Problem> Problems { get; set; } = [];
    }

    public class Problem
    {
        public List<long> Numbers { get; set; } = [];
        public char Operation { get; set; }
    }

    protected override Answer Part1()
    {
        return ProblemsTotal();
    }

    protected override Answer Part2()
    {
        return ProblemsTotal();
    }

    private long ProblemsTotal()
    {
        var total = 0L;
        foreach (var prob in Input.Problems)
        {
            if (prob.Operation == '+')
            {
                total += prob.Numbers.Sum();
            }
            else if (prob.Operation == '*')
            {
                total += prob.Numbers.Aggregate(1L, (acc, n) => acc * n);       // no .Product()
            }
            else
            {
                throw new Exception($"Unknown operation '{prob.Operation}'");
            }
        }

        return total;
    }

    protected override CephalopodMath Parse(RawInput input)
    {
        var lines = input.Lines().ToList();
        var problems = new List<Problem>();

        // part 2 is parsed completely differently.
        // numbers are read from right-to-left, top-to-bottom in columns.  the presence of an operation in that column means the problem is complete.
        // e.g.:
        // 64
        // 23
        // 314
        // +
        // should be read as 4 + 431 + 623
        if (IsPart2)
        {
            var pos = lines[0].Length-1;      // start at last column
            var numbers = new List<long>();

            while (pos >= 0)
            {
                var numStr = "";

                // read top-to-bottom in this column, reading out a continuous string of numbers (if any)
                var lineIdx = 0;
                while (lineIdx < lines.Count-1 && lines[lineIdx][pos] == ' ') lineIdx++;
                while (lineIdx < lines.Count-1 && char.IsDigit(lines[lineIdx][pos]))
                {
                    numStr += lines[lineIdx][pos];
                    lineIdx++;
                }
                if (numStr != "") numbers.Add(long.Parse(numStr));


                if (lines[^1][pos] == '+' || lines[^1][pos] == '*')
                {
                    // operation found, complete problem
                    problems.Add(new Problem
                    {
                        Numbers = numbers,
                        Operation = lines[^1][pos]
                    });
                    numbers = [];
                }

                pos--;
            }
        }
        else
        {
            // preallocate problems from first line
            var numberStrings = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var i=0; i < numberStrings.Length; i++)
            {
                problems.Add(new Problem());
            }

            for (var i=0; i < lines.Count - 1; i++)
            {
                numberStrings = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var numbers = numberStrings.Select(long.Parse).ToList();

                for (var j=0; j < numbers.Count; j++)
                {
                    problems[j].Numbers.Add(numbers[j]);
                }
            }

            // the last line is all of the operations
            var operations = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var j=0; j < operations.Length; j++)
            {
                var opChar = operations[j][0];
                problems[j].Operation = opChar;
            }
        }

        return new CephalopodMath
        {
            Problems = problems
        };
    }
}