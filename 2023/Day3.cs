namespace AOC.AOC2023;

public class Day3 : Day<Day3.Engine>
{
    protected override string? SampleRawInput { get => "467..114..\n...*......\n..35..633.\n......#...\n617*......\n.....+.58.\n..592.....\n......755.\n...$.*....\n.664.598.."; }

    public class Engine
    {
        public required List<PartNumber> Parts { get; set; }
        public required List<Symbol> Symbols { get; set; }
    }

    public class PartNumber
    {
        public int X { get; set; }
        public int EndX { get; set; }
        public int Y { get; set; }
        public int Number { get; set; }
    }

    public class Symbol
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Sym { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.Parts.Where(p => Input.Symbols.Any(q => q.X >= p.X-1 && q.X <= p.EndX+1 && q.Y >= p.Y-1 && q.Y <= p.Y+1))
            .Sum(p => p.Number);
    }

    protected override Answer Part2()
    {
        // the predicate is here twice but can't really be abstracted out because of the caller dependency.
        return Input.Symbols.Where(p => p.Sym == '*' && Input.Parts.Count(q => p.X >= q.X-1 && p.X <= q.EndX+1 && p.Y >= q.Y-1 && p.Y <= q.Y+1) == 2)
            .Select(p => Input.Parts.Where(q => p.X >= q.X-1 && p.X <= q.EndX+1 && p.Y >= q.Y-1 && p.Y <= q.Y+1))
            .Select(p => p.First().Number * p.Last().Number)        // .Count == 2
            .Sum();
    }

    protected override Engine Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();

        var grid = new char[lines.Length][];

        for (var y=0; y<lines.Length; y++)
        {
            grid[y] = new char[lines[y].Length];
            for (var x=0; x<lines[y].Length; x++)
            {
                grid[y][x] = lines[y][x];
            }
        }

        var nums = "0123456789";
        
        var parts = new List<PartNumber>();
        var symbols = new List<Symbol>();

        for (var y=0; y<grid.Length; y++)
        {
            var numStr = "";
            for (var x=0; x<grid[y].Length; x++)
            {
                if (nums.Contains(grid[y][x]))
                {
                    numStr += grid[y][x];
                }
                else
                {
                    if (grid[y][x] != '.')
                        symbols.Add(new Symbol() { X = x, Y = y, Sym = grid[y][x] });

                    if (numStr == "") continue;
                    parts.Add(new PartNumber() { X = x - numStr.Length, EndX = x-1, Y = y, Number = int.Parse(numStr) });
                    numStr = "";
                }
            }
            
            // don't forget a number going all the way to the end of the grid line!
            if (numStr != "")
                parts.Add(new PartNumber() { X = grid[y].Length - numStr.Length, EndX = grid[y].Length-1, Y = y, Number = int.Parse(numStr) });
        }

        return new Engine() { Parts = parts, Symbols = symbols };
    }
}

