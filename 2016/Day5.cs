using System.Security.Cryptography;
using System.Text;

namespace AOC.AOC2016;

public class Day5 : Day<Day5.SecurityDoor>
{
    protected override string? SampleRawInput { get => "abc"; }

    public class SecurityDoor
    {
        public required string DoorId { get; set; }

        public List<(char c, int pos)> PasswordChars = new List<(char, int)>();

        public string ComputePassword(bool part2 = false)
        {
            PasswordChars.Clear();
            var n=0;
            for (var i=0; i<8; i++)
            {
                while (true)
                {
                    n++;
                    var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{DoorId}{n}"));
                    if (hash[0] == 0 && hash[1] == 0 && hash[2] < 16 && (!part2 || (hash[2] < 8 && !PasswordChars.Any(p => p.pos == hash[2]))))
                    {
                        var pw = (part2 ? $"{hash[3]:x}" :$"{hash[2]:x}")[0];
                        var pos = part2 ? hash[2] : i;
                        System.Console.WriteLine(pw + " " + pos + " " + $"{hash[0]:x}{hash[1]:x}{hash[2]:x}{hash[3]:x}{hash[4]:x}");
                        PasswordChars.Add((pw, pos));
                        break;
                    }
                }
            }

            return part2 ? string.Join("", PasswordChars.OrderBy(p => p.pos).Select(p => p.c))
                : string.Join("", PasswordChars.Select(p => p.c));
        }
    }

    protected override Answer Part1()
    {
        return Input.ComputePassword();
    }

    protected override Answer Part2()
    {
        return Input.ComputePassword(true);
    }

    protected override SecurityDoor Parse(string input)
    {
        return new SecurityDoor() { DoorId = input };
    }
}