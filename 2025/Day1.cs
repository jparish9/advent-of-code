namespace AOC.AOC2025;

public class Day1 : Day<Day1.Instructions>
{
    protected override string? SampleRawInput { get => "L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82"; }

    public class Instructions
    {
        public required List<(char Turn, int Steps)> Moves;
    }

    protected override Answer Part1()
    {
        var pos = 50;
        var zeroes = 0;
        foreach (var (Turn, Steps) in Input.Moves)
        {
            if (Turn == 'L')
                pos -= Steps;
            else
                pos += Steps;
            
            while (pos < 0) pos += 100;
            while (pos >= 100) pos -= 100;

            if (pos == 0) zeroes++;
        }

        return zeroes;
    }

    protected override Answer Part2()
    {
        // it's only a few ms to do this iteratively instead of dealing with negative mod and edge cases.
        var pos = 50;
        var zeroClicks = 0;
        foreach (var (Turn, Steps) in Input.Moves)
        {
            for (var i=1; i<=Steps; i++)
            {
                if (Turn == 'L')
                    pos--;
                else
                    pos++;

                if (pos < 0) pos += 100;
                else if (pos >= 100) pos -= 100;

                if (pos == 0) zeroClicks++;
            }
        }

        return zeroClicks;
    }

    protected override Instructions Parse(RawInput input)
    {
        return new Instructions
        {
            Moves = [.. input.Lines().Select(line =>
                (line[0], int.Parse(line[1..])))]
        };
    }
}