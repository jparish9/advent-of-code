namespace AOC.AOC2024;

public class Day22 : Day<Day22.MonkeyMarket>
{
    protected override string? SampleRawInput { get => "1\n10\n100\n2024"; }
    protected override string? SampleRawInputPart2 { get => "1\n2\n3\n2024"; }

    public class MonkeyMarket
    {
        public required List<SecretNumber> SecretNumbers;
        public void Reset()
        {
            foreach (var secret in SecretNumbers)
            {
                secret.Reset();
            }
        }
    }

    public class SecretNumber(long n)
    {
        public void Reset()
        {
            Number = OrigNumber;
        }

        public long Number = n;
        public long OrigNumber = n;
        
        public long Iterate(int times)
        {
            for (var i=0; i<times; i++)
            {
                var op = Number << 6;       // * 64
                Number ^= op;
                Number &= 16777215;         // % 16777216

                op = Number >> 5;           // / 32
                Number ^= op;
                Number &= 16777215;

                op = Number << 11;          // * 2048
                Number ^= op;
                Number &= 16777215;
            }

            return Number;
        }

        // (price, [last 4 changes]), only return prices where we have >=4 previous changes; use a tuple so it can be a key
        public List<(int Price, (int Prev4, int Prev3, int Prev2, int Prev1) Changes)> Prices(int times)
        {
            var prices = new List<(int Price, (int, int, int, int) Changes)>();

            var changes = new List<int>();

            var last = (int)(Number % 10);

            for (var i=0; i<times; i++)
            {
                Iterate(1);
                var price = (int)(Number % 10);
                changes.Add(price - last);
                if (changes.Count >= 4) prices.Add((price, (changes[^4], changes[^3], changes[^2], changes[^1])));
                last = price;
            }

            return prices;
        }
    }

    protected override Answer Part1()
    {
        return Input.SecretNumbers.Aggregate(0L, (sum, secret) => sum + secret.Iterate(2000));
    }

    protected override Answer Part2()
    {
        Input.Reset();

        var allPrices = new List<List<(int Price, (int Prev4, int Prev3, int Prev2, int Prev1) Changes)>>();

        foreach (var secret in Input.SecretNumbers)
        {
            allPrices.Add(secret.Prices(2000));
        }

        // make a map of the last 4 changes and the FIRST price that appears for that sequence of changes
        var maps = new List<Dictionary<(int, int, int, int), int>>();

        foreach (var price in allPrices)
        {
            var mapItem = new Dictionary<(int, int, int, int), int>();
            foreach (var (Price, Changes) in price)
            {
                var key = (Changes.Prev4, Changes.Prev3, Changes.Prev2, Changes.Prev1);
                if (mapItem.ContainsKey(key)) continue;     // only the first one
                mapItem[key] = Price;
            }
            maps.Add(mapItem);
        }

        // from these maps, construct the overall map with the sum of the highest prices from each sequence
        var overall = new Dictionary<(int, int, int, int), int>();

        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (!overall.ContainsKey(key)) overall[key] = 0;
                overall[key] += value;
            }
        }

        return overall.Values.Max();
    }

    protected override MonkeyMarket Parse(string input)
    {
        return new MonkeyMarket { SecretNumbers = input.Split("\n").Where(p => p != "").Select(l => new SecretNumber(long.Parse(l))).ToList() };
    }
}