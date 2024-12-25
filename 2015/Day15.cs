namespace AOC.AOC2015;

public class Day15 : Day<Day15.Cookie>
{
    protected override string? SampleRawInput { get => "Butterscotch: capacity -1, durability -2, flavor 6, texture 3, calories 8\n"
        + "Cinnamon: capacity 2, durability 3, flavor -2, texture -1, calories 3"; }

    public class Cookie
    {
        public required List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public int Durability { get; set; }
        public int Flavor { get; set; }
        public int Texture { get; set; }
        public int Calories { get; set; }
    }

    protected override Answer Part1()
    {
        return BestCookie();
    }

    protected override Answer Part2()
    {
        return BestCookie((c) => c == 500);
    }

    private int BestCookie(Func<int, bool>? calorieCheck = null)
    {
        var ingredients = Input.Ingredients;
        var maxScore = 0;

        var split = new int[ingredients.Count];
        split[^1] = 100;

        while (split[0] <= 100)
        {
            var capacity = Math.Max(0, ingredients.Select((p, i) => (p, i)).Sum(q => q.p.Capacity * split[q.i]));
            var durability = Math.Max(0, ingredients.Select((p, i) => (p, i)).Sum(q => q.p.Durability * split[q.i]));
            var flavor = Math.Max(0, ingredients.Select((p, i) => (p, i)).Sum(q => q.p.Flavor * split[q.i]));
            var texture = Math.Max(0, ingredients.Select((p, i) => (p, i)).Sum(q => q.p.Texture * split[q.i]));

            if (calorieCheck == null || calorieCheck(ingredients.Select((p, i) => (p, i)).Sum(q => q.p.Calories * split[q.i])))
            {
                var score = capacity * durability * flavor * texture;
                maxScore = Math.Max(maxScore, score);
            }

            for (var i=ingredients.Count-2; i>=0; i--)
            {
                split[i]++;
                if (split[i] <= 100-split[0..i].Sum() || i == 0) break;
                split[i] = 0;
            }
            split[^1] = 100-split[0..^1].Sum();
        }

        return maxScore;
    }

    protected override Cookie Parse(RawInput input)
    {
        var ingredients = new List<Ingredient>();
        foreach (var line in input.Lines())
        {
            var parts = line.Split(' ');
            var name = parts[0].Trim(':');
            var capacity = int.Parse(parts[2].Trim(','));
            var durability = int.Parse(parts[4].Trim(','));
            var flavor = int.Parse(parts[6].Trim(','));
            var texture = int.Parse(parts[8].Trim(','));
            var calories = int.Parse(parts[10].Trim(','));

            ingredients.Add(new Ingredient { Name = name, Capacity = capacity, Durability = durability, Flavor = flavor, Texture = texture, Calories = calories });
        }

        return new Cookie { Ingredients = ingredients };
    }
}