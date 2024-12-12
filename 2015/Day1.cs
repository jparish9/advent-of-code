namespace AOC.AOC2015;

public class Day1 : Day<string>
{
    protected override string? SampleRawInput { get => "()())"; }

    protected override Answer Part1()
    {
        return Input.Aggregate(0, (floor, c) => floor + (c == '(' ? 1 : -1));
    }

    protected override Answer Part2()
    {
        var floor = 0;
        var pos = 0;
        while (floor != -1)
        {
            floor += Input[pos] == '(' ? 1 : -1;
            pos++;
        }

        return pos;     // 1-based
    }

    protected override string Parse(string input)
    {
        return input;
    }
}