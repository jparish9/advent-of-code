namespace AOC.AOC2024;

public class Day4 : Day<Day4.Grid>
{
    protected override string? SampleRawInput { get => "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX"; }

    public class Grid
    {
        public required char[][] Rows;

        public int RowCount;
        public int ColCount;

        public int FindXmas()
        {
            var found = 0;
            for (var sr=0; sr<RowCount; sr++)
            {
                for (var sc=0; sc<ColCount; sc++)
                {
                    if (Rows[sr][sc] != 'X') continue;

                    foreach (var vector in Vectors)
                    {
                        var r = sr;
                        var c = sc;
                        var ndx = 1;
                        while (ndx <= 3)
                        {
                            r += vector[0];
                            c += vector[1];
                            if (r < 0 || r >= RowCount || c < 0 || c >= ColCount
                                || Rows[r][c] != Xmas[ndx]) break;

                            ndx++;
                        }

                        if (ndx > 3) found++;           // did not break out of loop; found XMAS
                    }
                }
            }
            return found;
        }

        public int FindCrossXmas()
        {
            var found = 0;
            for (var sr=1; sr<RowCount-1; sr++)
            {
                for (var sc=1; sc<ColCount-1; sc++)
                {
                    if (Rows[sr][sc] != 'A') continue;

                    if (((Rows[sr-1][sc-1] == 'M' && Rows[sr+1][sc+1] == 'S')
                        || (Rows[sr-1][sc-1] == 'S' && Rows[sr+1][sc+1] == 'M'))
                        && ((Rows[sr-1][sc+1] == 'M' && Rows[sr+1][sc-1] == 'S')
                            || (Rows[sr-1][sc+1] == 'S' && Rows[sr+1][sc-1] == 'M')))
                    {
                        found++;
                    }
                }
            }
            return found;
        }
    }

    private static readonly List<int[]> Vectors = [ [0, 1], [1, 0], [0, -1], [-1, 0], [1, 1], [1, -1], [-1, 1], [-1, -1] ];

    private static readonly string Xmas = "XMAS";

    protected override Answer Part1()
    {
        return Input.FindXmas();
    }

    protected override Answer Part2()
    {
        return Input.FindCrossXmas();
    }

    protected override Grid Parse(string input)
    {
        var rows = input.Split('\n').Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
        return new Grid() { Rows = rows, RowCount = rows.Length, ColCount = rows[0].Length };
    }
}