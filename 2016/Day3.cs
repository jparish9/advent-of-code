namespace AOC.AOC2016;

public class Day3 : Day<Day3.Triangles>
{
    protected override string? SampleRawInput { get => "101 301 501\n102 302 502\n103 303 503\n201 401 601\n202 402 602\n203 403 603"; }

    public class Triangles
    {
        public required List<int[]> Triangle { get; set; }

        public int CountValid()
        {
            return Triangle.Count(p => p[0] + p[1] > p[2]);         // already sorted
        }
    }

    protected override long Part1()
    {
        return Input.CountValid();
    }

    protected override long Part2()
    {
        return Input.CountValid();
    }

    protected override Triangles Parse(string input)
    {
        var triangles = new List<int[]>();

        var cols = new List<List<int>>();

        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            var ln = Spaces().Replace(line.Trim(), " ").Split(' ');
            if (IsPart2)
            {
                cols.Add(new List<int> { int.Parse(ln[0].Trim()), int.Parse(ln[1].Trim()), int.Parse(ln[2].Trim()) });
                if (cols.Count == 3)
                {
                    triangles.Add(new List<int> { cols[0][0], cols[1][0], cols[2][0] }.OrderBy(p => p).ToArray());
                    triangles.Add(new List<int> { cols[0][1], cols[1][1], cols[2][1] }.OrderBy(p => p).ToArray());
                    triangles.Add(new List<int> { cols[0][2], cols[1][2], cols[2][2] }.OrderBy(p => p).ToArray());
                    cols.Clear();
                }
            }
            else
            {
                triangles.Add(new List<int> { int.Parse(ln[0].Trim()), int.Parse(ln[1].Trim()), int.Parse(ln[2].Trim()) }.OrderBy(p => p).ToArray());
            }
        }

        return new Triangles() { Triangle = triangles };
    }
}