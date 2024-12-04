namespace AOC.AOC2024;

public class Day4 : Day<Day4.Grid>
{
    protected override string? SampleRawInput { get => "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX"; }

    public class Grid
    {
        public required char[][] Rows;

        public int RowCount;
        public int ColCount;
    }

    private static readonly List<int[]> Vectors = new() { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0}, 
        new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, 1}, new int[] { -1, -1 } };

    private static readonly string Xmas = "XMAS";

    protected override long Part1()
    {
        var found = 0;
        for (var sr=0; sr<Input.RowCount; sr++)
        {
            for (var sc=0; sc<Input.ColCount; sc++)
            {
                if (Input.Rows[sr][sc] != 'X') continue;

                foreach (var vector in Vectors)
                {
                    var r = sr;
                    var c = sc;
                    var ndx = 1;
                    while (ndx <= 3)
                    {
                        r += vector[0];
                        c += vector[1];
                        if (r < 0 || r >= Input.RowCount || c < 0 || c >= Input.ColCount
                            || Input.Rows[r][c] != Xmas[ndx]) break;

                        ndx++;
                    }

                    if (ndx > 3) found++;           // did not break out of loop; found XMAS
                }
            }
        }
        return found;
    }

    protected override long Part2()
    {
        var found = 0;
        for (var sr=1; sr<Input.RowCount-1; sr++)
        {
            for (var sc=1; sc<Input.ColCount-1; sc++)
            {
                if (Input.Rows[sr][sc] != 'A') continue;

                if (((Input.Rows[sr-1][sc-1] == 'M' && Input.Rows[sr+1][sc+1] == 'S')
                    || (Input.Rows[sr-1][sc-1] == 'S' && Input.Rows[sr+1][sc+1] == 'M'))
                    && ((Input.Rows[sr-1][sc+1] == 'M' && Input.Rows[sr+1][sc-1] == 'S')
                        || (Input.Rows[sr-1][sc+1] == 'S' && Input.Rows[sr+1][sc-1] == 'M')))
                {
                    found++;
                }
            }
        }
        return found;
    }

    protected override Grid Parse(string input)
    {
        var rows = input.Split('\n').Where(p => p != "").Select(p => p.ToCharArray()).ToArray();
        return new Grid() { Rows = rows, RowCount = rows.Length, ColCount = rows[0].Length };
    }
}