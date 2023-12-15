namespace AOC.AOC2022;

public class Day2 : Day<List<Day2.Play>>
{
    protected override string? SampleRawInput { get => "A Y\nB X\nC Z"; }

    public class Play
    {
        public RPS MyPlay { get; set; }
        public RPS OppPlay { get; set; }
        public Instr Instruction { get; set; }            // for part 2
    }

    public enum RPS { Rock = 1, Paper, Scissors };
    public enum Instr { Lose, Draw, Win };              // for part 2

    protected override long Part1()
    {
        var score = 0;
        foreach (var line in Input)
        {
            score += (int)line.MyPlay;

            if (line.OppPlay == line.MyPlay) score += 3;
            else if ((line.OppPlay == RPS.Rock && line.MyPlay == RPS.Paper)
                || (line.OppPlay == RPS.Paper && line.MyPlay == RPS.Scissors)
                || (line.OppPlay == RPS.Scissors && line.MyPlay == RPS.Rock))
                score += 6;
        }

        return score;
    }

    protected override long Part2()
    {
        var score = 0;
        foreach (var line in Input)
        {
            if (line.Instruction == Instr.Draw) score += 3 + (int)line.OppPlay;
            else if (line.Instruction == Instr.Lose) {
                score += line.OppPlay == RPS.Rock ? (int)RPS.Scissors : line.OppPlay == RPS.Paper ? (int)RPS.Rock : (int)RPS.Paper;
            }
            else {      // win
                score += 6 + (line.OppPlay == RPS.Rock ? (int)RPS.Paper : line.OppPlay == RPS.Paper ? (int)RPS.Scissors : (int)RPS.Rock);
            }
        }

        return score;
    }

    protected override List<Play> Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();

        var result = new List<Play>();

        foreach (var line in lines)
        {
            var opp = line[0] == 'A' ? RPS.Rock : line[0] == 'B' ? RPS.Paper : RPS.Scissors;
            var mine = line[2] == 'X' ? RPS.Rock : line[2] == 'Y' ? RPS.Paper : RPS.Scissors;
            var instr = line[2] == 'X' ? Instr.Lose : line[2] == 'Y' ? Instr.Draw : Instr.Win;     // for part 2

            result.Add(new Play() { OppPlay = opp, MyPlay = mine, Instruction = instr });
        }

        return result;
    }
}