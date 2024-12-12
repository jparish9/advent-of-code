using System.Text.RegularExpressions;

namespace AOC.AOC2015;

public class Day8 : Day<List<string>>
{
    protected override string? SampleRawInput { get => @"""""" + "\n" + @"""abc""" + "\n" + @"""aaa\""aaa""" + "\n" + @"""\x27"""; }            // O_o

    protected override Answer Part1()
    {
        return Input.Select(p => p.Length - (Regex.Replace(p, @"\\x[0-9a-f]{2}", "a").Replace("\\\\", "a").Replace("\\\"", "a").Length - 2)).Sum();
    }

    protected override Answer Part2()
    {
        return Input.Select(p => p.Replace("\\", "\\\\").Replace("\"", "\\\"").Length + 2 - p.Length).Sum();        // + 2 to add back in literal surrounding quotes
    }

    protected override List<string> Parse(string input)
    {
        return @input.Split("\n").Where(p => p != "").ToList();         // @ for literal strings (ignore escape sequences like \\, \", \x24)
    }
}