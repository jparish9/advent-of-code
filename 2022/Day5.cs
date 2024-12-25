namespace AOC.AOC2022;

// the answers for this Day are strings; consider refactoring base class to support this.
public class Day5 : Day<Day5.StackMover>
{
    protected override string? SampleRawInput { get => "    [D]    \n[N] [C]    \n[Z] [M] [P]\n 1   2   3 \n\nmove 1 from 2 to 1\nmove 3 from 1 to 3\nmove 2 from 2 to 1\nmove 1 from 1 to 2"; }
    // protected override bool Part2ParsedDifferently => true;

    public class StackMover
    {
        public required List<Stack<char>> Stacks { get; set; }

        public required List<Instruction> Instructions { get; set; }
    }

    public class Instruction
    {
        public int Number { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }

    protected override Answer Part1()
    {
        // make a copy of the stacks so Parts don't interact
        var stacks = Input.Stacks.Select(p => new Stack<char>(p.Reverse())).ToList();

        foreach (var instruction in Input.Instructions)
        {
            var from = stacks[instruction.From - 1];
            var to = stacks[instruction.To - 1];

            for (var i=0; i<instruction.Number; i++)
            {
                to.Push(from.Pop());
            }
        }

        var result = "";
        foreach (var stack in stacks)
        {
            result += stack.Pop();
        }

        return result;
    }

    protected override Answer Part2()
    {
        // make a copy of the stacks so Parts don't interact
        var stacks = Input.Stacks.Select(p => new Stack<char>(p.Reverse())).ToList();

        foreach (var instruction in Input.Instructions)
        {
            var from = stacks[instruction.From - 1];
            var to = stacks[instruction.To - 1];

            var popped = new List<char>();
            for (var i=0; i<instruction.Number; i++)
            {
                popped.Add(from.Pop());
            }
            popped.Reverse();
            popped.ForEach(to.Push);
        }

        var result = "";
        foreach (var stack in stacks)
        {
            result += stack.Pop();
        }

        return result;
    }

    protected override StackMover Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();

        var stacks = new List<Stack<char>>();
        var instructions = new List<Instruction>();

        // find the line with just numbers, and allocate that many stacks
        var numberLine = 0;
        var numStacks = 0;
        for (;;numberLine++)
        {
            if (int.TryParse(lines[numberLine].Trim().Split(' ')[0], out int tmp))
            {
                numStacks = Spaces().Replace(lines[numberLine].Trim(), " ").Split(' ').Length;
                stacks.AddRange(Enumerable.Range(0, numStacks).Select(p => new Stack<char>()));
                break;
            }
        }

        // parse and populate the stacks
        for (var i=numberLine-1; i>=0; i--)
        {
            for (var j=1; j<numStacks*4; j += 4)
            {
                if (lines[i][j] == ' ') continue;
                stacks[j/4].Push(lines[i][j]);
            }
        }

        for (var i=numberLine+1; i<lines.Length; i++)
        {
            if (!lines[i].StartsWith("move")) continue;
            
            var line = lines[i].Split(' ').Where(p => p != "").ToArray();
            instructions.Add(new Instruction() { Number = int.Parse(line[1]), From = int.Parse(line[3]), To = int.Parse(line[5]) });
        }

        return new StackMover() { Stacks = stacks, Instructions = instructions };
    }
}