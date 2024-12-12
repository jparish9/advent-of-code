namespace AOC.AOC2022;

public class Day6 : Day<string>
{
    protected override string? SampleRawInput { get => "mjqjpqmgbljsphdztnvjfqwrcgsmlb"; }

    protected override Answer Part1()
    {
        return FindMarker(4);
    }

    protected override Answer Part2()
    {
        return FindMarker(14);
    }

    private long FindMarker(int length)
    {
        var i=0;

        for (; i<Input.Length; i++)
        {
            if (Input.Substring(i, length).Distinct().Count() == length)
            {
                break;
            }
        }

        return i+length;
    }

    protected override string Parse(string input)
    {
        return input;
    }
}