namespace AOC.AOC2016;

public class Day10 : Day<Day10.Factory>
{
    protected override string? SampleRawInput { get => "value 5 goes to bot 2\nbot 2 gives low to bot 1 and high to bot 0\nvalue 3 goes to bot 1\nbot 1 gives low to output 1 and high to bot 0\nbot 0 gives low to output 2 and high to output 0\nvalue 2 goes to bot 2"; }

    public class Factory
    {
        public required List<Instruction> Instructions;
        public required Dictionary<int, List<int>> Bots;
        public required Dictionary<int, List<int>> Outputs;

        public required bool IsSampleInput;

        public int ComparerBot = -1;            // for part 1

        public void Process()
        {
            // not explicitly stated, but exactly one bot will start with two chips.
            // also, there is exactly one instruction per bot.
            // finally, the instruction specifies what to do with BOTH chips.
            // thus, we can just process until no bots have two chips.
            while (true)
            {
                var bots = Bots.Where(p => p.Value.Count == 2).ToList();
                if (bots.Count == 0) break;

                foreach (var bot in bots)
                {
                    var instruction = Instructions.First(p => p.Bot == bot.Key);
                    var low = bot.Value.Min();
                    var high = bot.Value.Max();

                    if (IsSampleInput && low == 2 && high == 5)
                    {
                        ComparerBot = bot.Key;
                    }
                    else if (!IsSampleInput && low == 17 && high == 61)
                    {
                        ComparerBot = bot.Key;
                    }

                    if (instruction.LowDestination == Destination.Bot)
                    {
                        Bots[instruction.LowId].Add(low);
                    }
                    else
                    {
                        Outputs[instruction.LowId].Add(low);
                    }
                    if (instruction.HighDestination == Destination.Bot)
                    {
                        Bots[instruction.HighId].Add(high);
                    }
                    else
                    {
                        Outputs[instruction.HighId].Add(high);
                    }
                    bot.Value.Clear();
                }
            }
        }
    }

    public class Instruction
    {
        public int Bot;
        public Destination LowDestination;
        public int LowId;
        public Destination HighDestination;
        public int HighId;
    }

    public enum Destination
    {
        Bot,
        Output
    }

    protected override Answer Part1()
    {
        Input.Process();

        return Input.ComparerBot;
    }

    protected override Answer Part2()
    {
        return Input.Outputs[0][0] * Input.Outputs[1][0] * Input.Outputs[2][0];
    }

    protected override Factory Parse(RawInput input)
    {
        var instructions = new List<Instruction>();
        var bots = new Dictionary<int, List<int>>();
        var outputs = new Dictionary<int, List<int>>();

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" ");
            if (parts[0] == "value")
            {
                var value = int.Parse(parts[1]);
                var bot = int.Parse(parts[5]);
                if (!bots.ContainsKey(bot)) bots[bot] = [];
                bots[bot].Add(value);
            }
            else
            {
                var bot = int.Parse(parts[1]);
                if (!bots.ContainsKey(bot)) bots[bot] = [];         // ensure bot key if not yet defined

                var lowDest = parts[5] == "output" ? Destination.Output : Destination.Bot;
                var highDest = parts[10] == "output" ? Destination.Output : Destination.Bot;
                var lowId = int.Parse(parts[6]);
                var highId = int.Parse(parts[11]);

                // ensure output keys
                if (lowDest == Destination.Output && !outputs.ContainsKey(lowId)) outputs[lowId] = [];
                if (highDest == Destination.Output && !outputs.ContainsKey(highId)) outputs[highId] = [];

                instructions.Add(new Instruction() { Bot = bot, LowDestination = lowDest, LowId = lowId, HighDestination = highDest, HighId = highId });
            }
        }

        return new Factory() { Instructions = instructions, Bots = bots, Outputs = outputs, IsSampleInput = IsSampleInput };
    }
}