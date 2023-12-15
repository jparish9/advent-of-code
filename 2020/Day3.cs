namespace AOC.AOC2020;

public class Day3 : Day<char[][]>
{
    protected override string? SampleRawInput { get => "..##.......\n#...#...#..\n.#....#..#.\n..#.#...#.#\n.#...##..#.\n..#.##.....\n.#.#.#....#\n.#........#\n#.##...#...\n#...##....#\n.#..#...#.#"; }

    protected override long Part1()
    {
        return CountTrees(3, 1);
    }

    protected override long Part2()
    {
        return CountTrees(1, 1) * CountTrees(3, 1) * CountTrees(5, 1) * CountTrees(7, 1) * CountTrees(1, 2);
    }

    private long CountTrees(int xDir, int yDir)
    {
        var x = 0;
        var y = 0;
        var trees = 0L;

        while (y < Input.Length)
        {
            if (Input[y][x] == '#') trees++;

            x = (x + xDir) % Input[y].Length;
            y += yDir;
        }

        return trees;
    }

    protected override char[][] Parse(string input)
    {
        return input.Split('\n').Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
    }
}