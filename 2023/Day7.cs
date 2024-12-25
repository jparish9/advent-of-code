namespace AOC.AOC2023;

public class Day7 : Day<Day7.HandGroup>
{
    protected override string? SampleRawInput { get => "32T3K 765\nT55J5 684\nKK677 28\nKTJJT 220\nQQQJA 483"; }

    public enum HandRank { NotSet, HighCard, Pair, TwoPair, ThreeOfAKind, Flush, FullHouse, FourOfAKind, FiveOfAKind}

    public class HandGroup
    {
        public required List<Hand> Hands { get; set; }
        
        public long GetWinnings()
        {
            Hands.ForEach(h => h.Evaluate());
            Hands.Sort();

            var winnings = 0;
            for (var i=0; i<Hands.Count; i++)
            {
                winnings += Hands[i].Bid * (i+1);
            }

            return winnings;
        }
    }

    public class Hand : IComparable<Hand>
    {
        public required List<int> Cards { get; set; }

        public int Bid { get; set; }

        public HandRank Rank { get; set; } = HandRank.NotSet;

        public HandRank Evaluate()
        {
            Rank = HandRank.NotSet;

            // 0 are joker(s)
            var jokers = Cards.Count(p => p == 0);
            var nonJokers = Cards.Where(p => p != 0).ToList();

            if (jokers >= 4 || nonJokers.Distinct().Count() == 1) Rank = HandRank.FiveOfAKind;
            else if (nonJokers.Distinct().Count() == 2)
            {
                if (jokers + nonJokers.GroupBy(p => p).Max(q => q.Count()) == 4) Rank = HandRank.FourOfAKind;
                else Rank = HandRank.FullHouse;
            }
            else if (nonJokers.Distinct().Count() == 3)
            {
                if (jokers + nonJokers.GroupBy(p => p).Max(q => q.Count()) == 3) Rank = HandRank.ThreeOfAKind;
                else Rank = HandRank.TwoPair;
            }
            else if (jokers > 0 || nonJokers.Distinct().Count() == 4) Rank = HandRank.Pair;
            else Rank = HandRank.HighCard;

            return Rank;
        }

        public int CompareTo(Hand? other)
        {
            if (other == null) return 0;

            if (Rank != other.Rank) return Rank.CompareTo(other.Rank);

            // equal rank, compare cards first-to-last
            for (var i=0; i<Cards.Count; i++)
            {
                if (Cards[i] != other.Cards[i]) return Cards[i].CompareTo(other.Cards[i]);
            }

            return 0;
        }
    }

    // these have access to Input, which has been configured by RunPart*(true/false)
    protected override Answer Part1()
    {
        return Input.GetWinnings();
    }

    protected override Answer Part2()
    {
        return Input.GetWinnings();
    }

    protected override HandGroup Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();

        var hands = new List<Hand>();

        foreach (var line in lines)
        {
            var parts = line.Split(' ');

            hands.Add(new Hand()
            {
                Cards = parts[0].Select(p => p switch
                {
                    'T' => 10,
                    'J' => IsPart2 ? 0 : 11,            // for part 2, 'J' is a joker (not a jack), and has value 0 for comparing within ranks
                    'Q' => 12,
                    'K' => 13,
                    'A' => 14,
                    _ => int.Parse(p.ToString())
                }).ToList(),

                Bid = int.Parse(parts[1])
            });
        }

        return new HandGroup() { Hands = hands };
    }
}