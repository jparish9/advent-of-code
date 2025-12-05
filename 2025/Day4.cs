namespace AOC.AOC2025;

public class Day4 : Day<Day4.Grid>
{
    protected override string? SampleRawInput { get => "..@@.@@@@.\n@@@.@.@.@@\n@@@@@.@.@@\n@.@@@@..@.\n@@.@@@@.@@\n.@@@@@@@.@\n.@.@.@.@@@\n@.@@@.@@@@\n.@@@@@@@@.\n@.@.@@@.@."; }

    public class Grid(char[][] cells)
    {
        private readonly char[][] _cells = cells;
        private List<(int x, int y)> _accessibleCache = [];

        public List<(int x, int y)> AllAccessible()
        {
            _accessibleCache = [];
            for (var y=1; y < _cells.Length - 1; y++)
            {
                for (var x=1; x < _cells[y].Length - 1; x++)
                {
                    if (_cells[y][x] != '@') continue;

                    var rolls = 0;
                    for (var dx=-1; dx <= 1; dx++)
                    {
                        for (var dy=-1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            if (_cells[y+dy][x+dx] == '@') rolls++;
                        }
                    }

                    if (rolls < 4)
                    {
                        _accessibleCache.Add((x, y));
                    }
                }
            }

            return _accessibleCache;
        }

        public int RemoveAccessible()
        {
            _accessibleCache = AllAccessible();
            if (_accessibleCache.Count == 0) return 0;

            foreach (var (x, y) in _accessibleCache)
            {
                _cells[y][x] = '.';
            }
            return _accessibleCache.Count;
        }
    }

    protected override Answer Part1()
    {
        return Input.AllAccessible().Count;
    }

    protected override Answer Part2()
    {
        var totalRemoved = 0;
        int ct;
        while ((ct = Input.RemoveAccessible()) > 0) totalRemoved += ct;
        return totalRemoved;
    }

    protected override Grid Parse(RawInput input)
    {
        // return a padded grid with . around the input (needed for both parts, makes checking neighbors easier)
        var raw = input.Lines().ToArray();

        var padded = new char[raw.Length + 2][];
        padded[0] = new string('.', raw[0].Length + 2).ToCharArray();
        for (var i=0; i < raw.Length; i++)
        {
            padded[i + 1] = new char[raw[i].Length + 2];
            padded[i + 1][0] = '.';
            Array.Copy(raw[i].ToCharArray(), 0, padded[i + 1], 1, raw[i].Length);
            padded[i + 1][^1] = '.';
        }
        padded[^1] = new string('.', raw[0].Length + 2).ToCharArray();

        return new Grid(padded);
    }
}