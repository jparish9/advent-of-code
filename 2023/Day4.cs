namespace AOC.AOC2023;

public class Day4 : Day<List<Day4.Card>>
{
    protected override string? SampleRawInput { get => "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53\nCard 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19\nCard 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1\nCard 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83\nCard 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36\nCard 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"; }

    public class Card
    {
        public int Id { get; set; }
        public required int[] Mine { get; set; }
        public required int[] Winning { get; set; }
        public int Matches { get; set; }
    }

    protected override long Part1()
    {
        return (int)Input.Sum(p => p.Matches != 0 ? Math.Pow(2, p.Matches-1) : 0);
    }

    protected override long Part2()
    {
        var extras = new int[Input.Count];

        for (var i=0; i<Input.Count; i++)
        {
            // increment extras[i+1..i+matches+1] by one, plus one for each extra card we already have for the current one
            for (var j=i+1; j<i+Input[i].Matches+1; j++)
            {
                extras[j] += 1 + extras[i];
            }
        }

        return Input.ToArray().Select((p, i) => 1 + extras[i]).Sum();
    }

    protected override List<Card> Parse(string input)
    {
        var cards = new List<Card>();
        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            var parts = line.Split(':');
            var id = int.Parse(Spaces().Replace(parts[0], " ").Split(' ')[1]);
            var mine  = Spaces().Replace(parts[1].Split('|')[0].Trim(), " ").Split(' ').Select(int.Parse).ToArray();
            var winning = Spaces().Replace(parts[1].Split('|')[1].Trim(), " ").Split(' ').Select(int.Parse).ToArray();
            var matches = mine.Intersect(winning).Count();
            
            cards.Add(new Card() { Id = id, Mine = mine, Winning = winning, Matches = matches });
        }

        return cards;
    }
}