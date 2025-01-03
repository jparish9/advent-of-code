using System.Text;

namespace AOC.AOC2015;

public class Day10 : Day<string>
{
    protected override string? SampleRawInput { get => "1"; }

    protected override Answer Part1()
    {
        return LookAndSay(Input, 40);
    }

    protected override Answer Part2()
    {
        return LookAndSay(Input, 50);
    }

    private static int LookAndSay(string input, int count)
    {
        var str = input;
        for (int i = 0; i < count; i++)
        {
            str = LookAndSay(str);
        }

        return str.Length;
    }

    private static string LookAndSay(string input)
    {
        var sb = new StringBuilder();

        int count = 0;
        char last = input[0];

        foreach (var c in input)
        {
            if (c == last)
            {
                count++;
            }
            else
            {
                sb.Append(count);
                sb.Append(last);
                count = 1;
                last = c;
            }
        }

        sb.Append(count);
        sb.Append(last);

        return sb.ToString();
    }

    protected override string Parse(RawInput input)
    {
        return input;
    }
}