namespace AOC.AOC2023;

public class Day15 : Day<List<Day15.Step>>
{
    protected override string? SampleRawInput { get => "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"; }

    public class Step
    {
        public required string BoxLabel { get; set; }
        public required char Instruction { get; set; }
        public int? FocalLength { get; set; }

        public int Hash()
        {
            return Hash(BoxLabel + Instruction + FocalLength);
        }

        public int HashLabel()
        {
            return Hash(BoxLabel);
        }

        private static int Hash(string str)
        {
            return str.Aggregate(0, (acc, c) => (acc + c) * 17 % 256);
        }
    }

    protected override Answer Part1()
    {
        return Input.Sum(p => p.Hash());
    }

    protected override Answer Part2()
    {
        var boxes = new List<List<Step>>();
        for (var i=0; i<256; i++) { boxes.Add(new List<Step>()); }

        foreach (var step in Input)
        {
            var hash = step.HashLabel();
            var box = boxes[hash];
            var index = box.FindIndex(p => p.BoxLabel == step.BoxLabel);

            if (step.Instruction == '=')
            {
                if (index >= 0)
                    box[index].FocalLength = step.FocalLength;
                else
                    box.Add(step);
            }
            else if (index >= 0)            // step.Instruction == '-'
            {
                box.RemoveAt(index);
            }
        }

        var totalFocusPower = 0;
        for (var i=0; i<boxes.Count; i++)
        {
            for (var j=0; j<boxes[i].Count; j++)
            {
                totalFocusPower += (i+1) * (j+1) * (int)boxes[i][j].FocalLength!;
            }
        }

        return totalFocusPower;
    }

    protected override List<Step> Parse(RawInput input)
    {
        return input.Value.Replace("\n", "").Split(',').Where(p => p != "").Select(p =>
        {
            var c = 0;
            while (p[c] != '=' && p[c] != '-') { c++; }
            return new Step()
            {
                BoxLabel = p[..c],
                Instruction = p[c],
                FocalLength = p.Length > c+1 ? int.Parse(p[(c + 1)..]) : null
            };
        }).ToList();

    }
}