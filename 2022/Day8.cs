using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace AOC.AOC2022;

public class Day8 : Day<List<Day8.Tree>>
{
    protected override string? SampleRawInput { get => "30373\n25512\n65332\n33549\n35390"; }

    public class Tree
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }

        // heights of neighbor trees in the 4 directions, ordered by closest first
        public List<List<int>> OrderedNeighborHeights { get; set; } = new();

        public bool IsVisible()
        {
            return OrderedNeighborHeights.Any(p => p.Count == 0 || p.All(q => q < Height));
        }

        public int ScenicScore()
        {
            var scenicScore = 1;
            foreach (var dir in OrderedNeighborHeights)
            {
                if (dir.Count == 0) { scenicScore = 0; break; }

                var thisDirScore = 0;
                for (var i=0; i<dir.Count; i++)
                {
                    thisDirScore++;
                    if (dir[i] >= Height) break;
                }

                scenicScore *= thisDirScore;
            }

            return scenicScore;
        }
    }

    protected override Answer Part1()
    {
        return Input.Count(p => p.IsVisible());
    }

    protected override Answer Part2()
    {
        return Input.Max(p => p.ScenicScore());
    }

    protected override List<Tree> Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();

        var result = new List<Tree>();
        var trees = new int[lines.Length][];

        for (var i=0; i<lines.Length; i++)
        {
            trees[i] = new int[lines[i].Length];
            for (var j=0; j<lines[i].Length; j++)
            {
                var height = int.Parse(lines[i][j].ToString());
                trees[i][j] = height;
                result.Add(new Tree { X = j, Y = i, Height = height });
            }
        }

        // for each tree, store its neighbors' heights in all 4 directions, as we need them for both parts.
        foreach (var tree in result)
        {
            foreach (var (X, Y) in new []{ (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var x = tree.X;
                var y = tree.Y;

                var orderedHeights = new List<int>();

                while (true)
                {
                    x += X;
                    y += Y;

                    if (x < 0 || x >= trees[0].Length || y < 0 || y >= trees.Length) break;
                    orderedHeights.Add(trees[y][x]);
                }

                tree.OrderedNeighborHeights.Add(orderedHeights);
            }
        }

        return result;
    }
}