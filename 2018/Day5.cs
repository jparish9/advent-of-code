using System.Text;

namespace AOC.AOC2018;

public class Day5 : Day<Day5.Polymer>
{
    protected override string? SampleRawInput { get => "dabAcCaCBAcCcaDA"; }

    public class Polymer
    {
        public required string Units;

        public string Reduce()
        {
            return Reduce(Units);
        }

        public static string Reduce(string polymer)
        {
            var units = new StringBuilder(polymer);
            var i = 0;
            while (i < units.Length - 1)
            {
                if (Math.Abs(units[i] - units[i + 1]) == 32)            // Abs('a' - 'A')
                {
                    units = units.Remove(i, 2);         // remove the pair
                    i = Math.Max(0, i - 1);             // go back one unit
                }
                else i++;                               // not a pair, advance
            }

            return units.ToString();
        }
    }

    protected override Answer Part1()
    {
        return Input.Reduce().Length;
    }

    protected override Answer Part2()
    {
        var min = int.MaxValue;
        for (var i = 65; i <= 90; i++)      // A-Z
        {
            var units = Input.Units;

            units = units.Replace(((char)i).ToString(), "");            // remove capital
            units = units.Replace(((char)(i + 32)).ToString(), "");     // remove matching lowercase

            units = Polymer.Reduce(units);

            min = Math.Min(min, units.Length);
        }

        return min;
    }

    protected override Polymer Parse(string input)
    {
        return new Polymer() { Units = input.Replace("\n", "") };
    }
}