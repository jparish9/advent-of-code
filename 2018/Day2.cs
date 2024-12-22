namespace AOC.AOC2018;

public class Day2 : Day<Day2.Boxes>
{
    protected override string? SampleRawInput { get => "abcdef\nbababc\nabbcde\nabcccd\naabcdd\nabcdee\nababab"; }
    protected override string? SampleRawInputPart2 { get => "abcde\nfghij\nklmno\npqrst\nfguij\naxcye\nwvxyz"; }

    public class Boxes
    {
        public required List<string> BoxIds;
    }

    protected override Answer Part1()
    {
        var twos = 0;
        var threes = 0;
        foreach (var boxId in Input.BoxIds)
        {
            var counts = boxId.GroupBy(p => p).Select(p => p.Count()).ToList();
            if (counts.Contains(2)) twos++;
            if (counts.Contains(3)) threes++;
        }

        return twos * threes;
    }

    protected override Answer Part2()
    {
        string? result = null;
        foreach (var box1 in Input.BoxIds)
        {
            foreach (var box2 in Input.BoxIds)
            {
                if (box1 == box2) continue;

                var diff = 0;
                var common = "";
                for (var i = 0; i < box1.Length; i++)
                {
                    if (box1[i] != box2[i])
                    {
                        diff++;
                        if (diff > 1) break;
                    }
                    else
                    {
                        common += box1[i];
                    }
                }

                if (diff == 1)
                {
                    result = common;
                    break;
                }
            }
            if (result != null) break;
        }

        return result!;
    }

    protected override Boxes Parse(string input)
    {
        return new Boxes() { BoxIds = input.Split("\n").Where(p => p != "").ToList() };
    }
}