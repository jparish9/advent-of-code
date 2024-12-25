namespace AOC.AOC2023;

public class Day2 : Day<List<Day2.Game>>
{
    protected override string SampleRawInput { get => "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green\nGame 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue\nGame 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red\nGame 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red\nGame 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"; }

    public class Game
    {
        public int Id { get; set; }

        public required Dictionary<string, int> Maxes { get; set; }
    }

    protected override Answer Part1()
    {
        var constraints = new Dictionary<string, int>() {{"red", 12}, {"green", 13}, {"blue", 14}};

        return Input.Sum(p => p.Maxes.All(q => q.Value <= constraints[q.Key]) ? p.Id : 0);
    }

    protected override Answer Part2()
    {
        var pwr = 0.0;

        foreach (var game in Input)
        {
            var maxes = new Dictionary<string, int>() {{"red", 0}, {"green", 0}, {"blue", 0}};
            game.Maxes.ToList().ForEach(p => maxes[p.Key] = Math.Max(maxes[p.Key], p.Value));

            pwr += Math.Exp(maxes.Sum(p => Math.Log(p.Value)));     // .Product() is not a thing, but .Exp(.Sum(.Log())) does the same thing
        }

        return (int)pwr;
    }

    protected override List<Game> Parse(RawInput input)
    {
        var games = new List<Game>();
        foreach (var line in input.Lines())
        {
            var gameId = int.Parse(line.Split(':')[0].Split(" ")[1]);
            var gamePlays = line.Split(':')[1].Trim().Split(';').Select(p => p.Trim().Replace(",","").Split(' ')).ToArray();
            var maxes = new Dictionary<string, int>() {{"red", 0}, {"green", 0}, {"blue", 0}};

            foreach (var item in gamePlays)
            {
                for(var i=0; i<item.Length; i+=2)
                {
                    var color = item[i+1];
                    maxes[color] = Math.Max(maxes[color], int.Parse(item[i]));
                }
            }

            games.Add(new Game() { Id = gameId, Maxes = maxes });
        }

        return games;
    }
}