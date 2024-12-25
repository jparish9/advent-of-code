using System.Text.RegularExpressions;

namespace AOC.AOC2016;

public class Day7 : Day<Day7.IPV7>
{
    protected override string? SampleRawInput { get => "abba[mnop]qrst\nabcd[bddb]xyyx\naaaa[qwer]tyui\nioxxoj[asdfgh]zxcvbn"; }

    public class IPV7
    {
        public required List<string> IPs;
    }

    protected override Answer Part1()
    {
        var tls = 0;
        foreach (var ip in Input.IPs)
        {
            // probably a way to do this with regex, but this is much more readable.

            var abba = false;
            var hypernet = false;
            for (var i=0; i<ip.Length-3; i++)
            {
                if (ip[i] == '[') hypernet = true;
                if (ip[i] == ']') hypernet = false;
                if (ip[i] == ip[i+3] && ip[i+1] == ip[i+2] && ip[i] != ip[i+1])
                {
                    if (hypernet)
                    {
                        abba = false;
                        break;
                    }
                    abba = true;            // found, but can be disqualified by subsequent hypernet with ABBA
                }
            }

            if (abba) tls++;
        }

        return tls;
    }

    protected override Answer Part2()
    {
        var ssl = 0;
        foreach (var ip in Input.IPs)
        {
            var abas = new List<string>();
            var babs = new List<string>();
            var hypernet = false;
            for (var i=0; i<ip.Length-2; i++)
            {
                if (ip[i] == '[') hypernet = true;
                if (ip[i] == ']') hypernet = false;
                if (ip[i] == ip[i+2] && ip[i] != ip[i+1])
                {
                    if (hypernet) babs.Add($"{ip[i+1]}{ip[i]}{ip[i+1]}");           // save the BAB as ABA so we can compare below
                    else abas.Add($"{ip[i]}{ip[i+1]}{ip[i]}");
                }
            }

            if (abas.Any(aba => babs.Any(bab => aba == bab))) ssl++;
        }

        return ssl;
    }

    protected override IPV7 Parse(RawInput input)
    {
        return new IPV7() { IPs = input.Lines().ToList() };
    }
}