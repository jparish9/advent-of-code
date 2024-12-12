using System.Text.RegularExpressions;

namespace AOC.AOC2024;

public partial class Day3 : Day<Day3.Instructions>
{
    protected override string? SampleRawInput { get => "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"; }
    protected override string? SampleRawInputPart2 { get => "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5));"; }

    public class Instructions
    {
        public required List<Instruction> Items;

    }

    public class Instruction
    {
        public required string Command;         // mul, do, don't

        public required List<int> Args;         // empty for do, don't; 2 integers for mul
    }

    protected override Answer Part1()
    {
        return Input.Items.Where(p => p.Command == "mul").Sum(p => p.Args[0] * p.Args[1]);
    }

    protected override Answer Part2()
    {
        var enabled = true;
        var result = 0;
        foreach (var instruction in Input.Items)
        {
            switch (instruction.Command)
            {
                case "do":
                    enabled = true;
                    break;
                case "don't":
                    enabled = false;
                    break;
                case "mul":
                    if (enabled)
                        result += instruction.Args[0] * instruction.Args[1];
                    break;
            }
        }
        return result;
    }

    protected override Instructions Parse(string input)
    {
        return new Instructions() {
            Items = InstructionMatch().Matches(input)
                .Select(p => new Instruction() { Command = p.Groups[1].Value, Args = p.Groups[1].Value == "mul"
                    ? new List<int>() { int.Parse(p.Groups[2].Value), int.Parse(p.Groups[3].Value) }            // save arguments for mul
                    : new List<int>() }).ToList() };
    }

    // match mul(a,b), do(), don't()
    [GeneratedRegex("(mul|do|don't)\\((?:\\)|(\\d+),(\\d+)\\))")]
    private static partial Regex InstructionMatch();
}