using System.Security.Cryptography;
using System.Text;

namespace AOC.AOC2016;

public class Day5 : Day<Day5.SecurityDoor>
{
    protected override string? SampleRawInput { get => "abc"; }

    public class SecurityDoor
    {
        public required string DoorId { get; set; }

        public char[] PasswordChars = new char[8];

        public string ComputePassword(bool part2 = false)
        {
            PasswordChars = new char[8];
            var n=0;
            for (var i=0; i<8; i++)
            {
                while (true)
                {
                    n++;
                    var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{DoorId}{n}"));
                    if (hash[0] == 0 && hash[1] == 0 && hash[2] < 16 && (!part2 || (hash[2] < 8 && PasswordChars[hash[2]] == 0)))
                    {
                        var pw = (part2 ? $"{hash[3]/16:x}" :$"{hash[2]:x}")[0];
                        var pos = part2 ? hash[2] : i;
                        PasswordChars[pos] = pw;
                        break;
                    }
                }
            }

            return string.Join("", PasswordChars);
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

    protected override SecurityDoor Parse(RawInput input)
    {
        return new SecurityDoor() { DoorId = input };
    }
}