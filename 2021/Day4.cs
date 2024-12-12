namespace AOC.AOC2021;

public class Day4 : Day<Day4.Bingo>
{
    protected override string? SampleRawInput { get => "7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1\n\n22 13 17 11  0\n 8  2 23  4 24\n21  9 14 16  7\n 6 10  3 18  5\n 1 12 20 15 19\n\n 3 15  0  2 22\n 9 18 13 17  5\n19  8  7 25 23\n20 11 10 24  4\n14 21 16 12  6\n\n14 21 17 24  4\n10 16 15  9 19\n18  8 23 26 20\n22 11 13  6  5\n 2  0 12  3  7"; }

    public class Bingo
    {
        public required List<int> Numbers { get; set; }

        public required List<Card> Cards { get; set; }

    }

    public class Card
    {
        public required int[][] Grid { get; set; }
        public required bool[][] Marked { get; set; }

        public void Mark(int number)
        {
            for (var i=0; i<Grid.Length; i++)
            {
                for (var j=0; j<Grid[i].Length; j++)
                {
                    if (Grid[i][j] == number)
                    {
                        Marked[i][j] = true;
                        return;
                    }
                }
            }
        }

        public void Clear()
        {
            for (var i=0; i<Grid.Length; i++)
            {
                for (var j=0; j<Grid[i].Length; j++)
                {
                    Marked[i][j] = false;
                }
            }
        }

        public int SumUnmarked()
        {
            var sum = 0;
            for (var i=0; i<Grid.Length; i++)
            {
                for (var j=0; j<Grid[i].Length; j++)
                {
                    if (!Marked[i][j]) sum += Grid[i][j];
                }
            }
            return sum;
        }

        public bool IsBingo()
        {
            var isBingo = false;
            // check rows
            if (Marked.Any(p => p.All(q => q))) return true;

            // check columns
            for (var i=0; i<Marked.Length; i++)
            {
                isBingo = true;
                for (var j=0; j<Marked[i].Length; j++)
                {
                    if (!Marked[j][i]) { isBingo = false; break; }
                }
                if (isBingo) return true;
            }

            return isBingo;
        }
    }

    protected override Answer Part1()
    {
        var i=0;
        Card? winning = null;
        Input.Cards.ForEach(p => p.Clear());

        for (i=0; i<Input.Numbers.Count; i++)
        {
            Input.Cards.ForEach(p => p.Mark(Input.Numbers[i]));

            winning = Input.Cards.FirstOrDefault(p => p.IsBingo());
            if (winning != null) break;
        }

        return winning!.SumUnmarked() * Input.Numbers[i];
    }

    protected override Answer Part2()
    {
        var i=0;
        Card? winning = null;
        Input.Cards.ForEach(p => p.Clear());

        // make copy of Input so we can remove winning cards
        var cards = Input.Cards.Select(p => new Card() { Grid = p.Grid, Marked = p.Marked.Select(q => q.ToArray()).ToArray() }).ToList();

        for (i=0; i<Input.Numbers.Count; i++)
        {
            cards.ForEach(p => p.Mark(Input.Numbers[i]));

            if (cards.Count == 1 && cards.First().IsBingo())
            {
                winning = cards.First();
                break;
            }

            // remove winning cards
            cards.RemoveAll(p => p.IsBingo());
        }

        return cards.First().SumUnmarked() * Input.Numbers[i];
    }

    protected override Bingo Parse(string input)
    {
        var parts = input.Split("\n\n");

        var numbers = parts[0].Split(',').Select(int.Parse).ToList();
        var cards = parts[1..].Select(p => p.Split('\n').Where(p => p != "").Select(q => Spaces().Replace(q, " ").Split(' ').Where(r => r != "").Select(int.Parse).ToArray()).ToArray()).ToList();
        var marked = cards.Select(p => p.Select(q => q.Select(r => false).ToArray()).ToArray()).ToList();

        return new Bingo() { Numbers = numbers, Cards = cards.Select((p, i) => new Card() { Grid = p, Marked = marked[i] }).ToList() };
    }
}