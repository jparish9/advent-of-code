using System.Text.RegularExpressions;

namespace AOC.AOC2018;

public partial class Day3 : Day<Day3.Claims>
{
    protected override string? SampleRawInput { get => "#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2"; }

    public class Claims
    {
        public required List<Claim> Items;

        public Dictionary<(int X, int Y), (int claim1, int claim2)> Overlapping = [];              // cache

        public void ComputeOverlapping()
        {
            foreach (var claim in Items)
            {
                foreach (var claim2 in Items)
                {
                    if (claim.Id == claim2.Id) continue;

                    for (var x = Math.Max(claim.Position.X, claim2.Position.X); x < Math.Min(claim.Position.X + claim.Width, claim2.Position.X + claim2.Width); x++)
                    {
                        for (var y = Math.Max(claim.Position.Y, claim2.Position.Y); y < Math.Min(claim.Position.Y + claim.Height, claim2.Position.Y + claim2.Height); y++)
                        {
                            if (Overlapping.ContainsKey((x, y))) continue;
                            Overlapping.Add((x, y), (claim.Id, claim2.Id));
                        }
                    }
                }
            }
        }
    }

    public class Claim
    {
        public required int Id;
        public required (int X, int Y) Position;
        public required int Width;
        public required int Height;
    }

    protected override Answer Part1()
    {
        Input.ComputeOverlapping();
        return Input.Overlapping.Count;
    }

    protected override Answer Part2()
    {
        // all overlapping squares with the two claim ids cached from part 1.
        var overlapping = Input.Overlapping.Select(p => p.Value.claim1).Concat(Input.Overlapping.Select(p => p.Value.claim2)).ToHashSet();
        return Input.Items.First(p => !overlapping.Contains(p.Id)).Id;
    }

    protected override Claims Parse(string input)
    {
        return new Claims() {
            Items = input.Split("\n").Where(p => p != "").Select(p => ClaimMatch().Matches(p)).Select(p => new Claim() {
                Id = int.Parse(p[0].Groups[1].Value),
                Position = (int.Parse(p[0].Groups[2].Value), int.Parse(p[0].Groups[3].Value)),
                Width = int.Parse(p[0].Groups[4].Value),
                Height = int.Parse(p[0].Groups[5].Value)
            }
        ).ToList() };
    }

    [GeneratedRegex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")]
    private static partial Regex ClaimMatch();
}