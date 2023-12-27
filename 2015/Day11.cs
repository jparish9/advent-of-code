using System.Text;

namespace AOC.AOC2015;

public class Day11 : Day<string>
{
    protected override string? SampleRawInput { get => "ghijklmn"; }

    protected override long Part1()
    {
        System.Console.WriteLine(NextPassword(Input));
        return 0;           // not a numeric answer
    }

    protected override long Part2()
    {
        System.Console.WriteLine(NextPassword(NextPassword(Input)));
        return 0;           // not a numeric answer
    }

    private static string NextPassword(string password)
    {
        do
        {
            password = Increment(password);
        } while (!IsValid(password));

        return password;
    }

    private static string Increment(string password)
    {
        var sb = new StringBuilder(password);

        for (int i = password.Length-1; i >= 0; i--)
        {
            if (password[i] == 'z')         // roll over z to a
            {
                sb[i] = 'a';
            }
            else
            {
                sb[i] = (char)(password[i] + 1);         // increment last and break, or "carry the 1" and break
                break;
            }
        }

        return sb.ToString();
    }

    private static bool IsValid(string password)
    {
        if (password.Contains('i') || password.Contains('o') || password.Contains('l')) return false;

        // check for "straight" like cde, xyz, etc
        bool foundStraight = false;
        for (var i=0; i<password.Length-2; i++)
        {
            if (password[i] == password[i+1] - 1 && password[i] == password[i+2] - 2)
            {
                foundStraight = true;
                break;
            }
        }
        if (!foundStraight) return false;

        // check for at least two pairs, separated by another letter, like bbacc
        var pairs = 0;
        for (var i=0; i<password.Length-1; i++)
        {
            if (password[i] == password[i + 1])
            {
                pairs++;
                i++;            // skip next char; "bbb" should not count as two straights
                if (pairs >= 2) break;
            }
        }

        return pairs >= 2;
    }

    protected override string Parse(string input)
    {
        return input;
    }
}