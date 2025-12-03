namespace AOC.AOC2025;

public class Day3 : Day<Day3.BatteryBanks>
{
    protected override string? SampleRawInput { get => "987654321111111\n811111111111119\n234234234234278\n818181911112111"; }

    public class BatteryBanks
    {
        public List<List<int>> Banks = [];

    }

    protected override Answer Part1()
    {
        return Joltage(2);
    }

    protected override Answer Part2()
    {
        return Joltage(12);
    }

    private long Joltage(int batteries)
    {
        var totalMax = 0L;
        foreach (var bank in Input.Banks)
        {
            var minIdx = 0;
            var num = "";
            for (var i=batteries-1; i>=0; i--)
            {
                var max = bank[minIdx..(bank.Count-i)].Max();           // find maximium of digits starting after last one that leaves enough left
                var maxIdx = bank[minIdx..].IndexOf(max);
                num += bank[minIdx+maxIdx].ToString();

                minIdx += maxIdx + 1;           // update starting pointer
            }

            totalMax += long.Parse(num);
        }

        return totalMax;
    }

    protected override BatteryBanks Parse(RawInput input)
    {
        var banks = new BatteryBanks();
        foreach (var line in input.Lines())
        {
            banks.Banks.Add([.. line.Select(c => int.Parse(c.ToString()))]);
        }
        return banks;
    }
}