namespace AOC.AOC2020;

public class Day1 : Day<List<int>>
{
    protected override string? SampleRawInput { get => "1721\n979\n366\n299\n675\n1456"; }

    protected override Answer Part1()
    {
        var product = -1;
        // find 2 entries that sum to 2020
        for (var i=0; i<Input.Count; i++)
        {
            for (var j=i+1; j<Input.Count; j++)
            {
                if (Input[i] + Input[j] == 2020)
                {
                    product = Input[i] * Input[j];
                    break;
                }
            }
        }

        return product;
    }

    protected override Answer Part2()
    {
        // for a much larger input set, sort and binary search could probably be used, but this is more than fast enough at ~10ms
        var product = -1;
        for (var i=0; i<Input.Count; i++)
        {
            for (var j=i+1; j<Input.Count; j++)
            {
                for (var k=j+1; k<Input.Count; k++)
                {
                    if (Input[i] + Input[j] + Input[k] == 2020)
                    {
                        product = Input[i] * Input[j] * Input[k];
                        break;
                    }
                }
            }
        }

        return product;
    }

    protected override List<int> Parse(string input)
    {
        return input.Split('\n').Where(p => p != "").Select(int.Parse).OrderBy(p => p).ToList();
    }
}