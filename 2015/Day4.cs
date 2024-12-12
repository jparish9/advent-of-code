namespace AOC.AOC2015;

public class Day4 : Day<string>
{
    protected override string? SampleRawInput { get => ""; }

    protected override Answer Part1()
    {
        if (Input == "") return 0;          // ignore no sample

        var i=0;
        for (; i < int.MaxValue; i++)
        {
            var hash = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.ASCII.GetBytes($"{Input}{i}"));
            if (hash[0] == 0 && hash[1] == 0 && hash[2] < 16) break;        // first 3 hex bytes at most 15 ==> 00000... in hex
        }

        return i;
    }

    protected override Answer Part2()
    {
        if (Input == "") return 0;          // ignore no sample

        var i=0;
        for (; i < int.MaxValue; i++)
        {
            var hash = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.ASCII.GetBytes($"{Input}{i}"));
            if (hash[0] == 0 && hash[1] == 0 && hash[2] == 0) break;        // first 3 hex bytes are 0 ==> 000000... in hex
        }

        return i;
    }

    protected override string Parse(string input)
    {
        return input;
    }
}