namespace AOC.AOC2016;

public class Day8 : Day<Day8.Screen>
{
    protected override string? SampleRawInput { get => "rect 3x2\nrotate column x=1 by 4\nrotate row y=0 by 4\nrotate column x=1 by 1"; }

    public class Screen
    {
        public required List<Instruction> Instructions;
        public char[][] Pixels = new char[6][];

        public void Reset()
        {
            for (var i=0; i<Pixels.Length; i++)
            {
                Pixels[i] = Enumerable.Repeat('.', 50).ToArray();
            }
        }

        public string Print()
        {
            return "\n" + Pixels.Select(p => new string(p)).Aggregate((a, b) => $"{a}\n{b}");
        }

        public int PixelsOn()
        {
            return Pixels.SelectMany(p => p).Count(p => p == '#');
        }
    }

    public abstract class Instruction
    {
        public abstract void Apply(char[][] pixels);
    }

    public class Rect : Instruction
    {
        public required int Width;
        public required int Height;

        public override void Apply(char[][] pixels)
        {
            for (var y=0; y<Height; y++)
            {
                for (var x=0; x<Width; x++)
                {
                    pixels[y][x] = '#';
                }
            }
        }
    }

    public class RotateRow : Instruction
    {
        public required int Row;
        public required int Pixels;

        public override void Apply(char[][] pixels)
        {
            var row = pixels[Row];
            var newRow = new char[row.Length];
            for (var i=0; i<row.Length; i++)
            {
                newRow[(i+Pixels) % row.Length] = row[i];
            }

            pixels[Row] = newRow;
        }
    }

    public class RotateColumn : Instruction
    {
        public required int Column;
        public required int Pixels;

        public override void Apply(char[][] pixels)
        {
            var column = new char[pixels.Length];
            for (var i=0; i<pixels.Length; i++)
            {
                column[(i+Pixels) % pixels.Length] = pixels[i][Column];
            }

            for (var i=0; i<pixels.Length; i++)
            {
                pixels[i][Column] = column[i];
            }
        }
    }

    protected override Answer Part1()
    {
        Input.Reset();
        foreach (var instruction in Input.Instructions)
        {
            instruction.Apply(Input.Pixels);
        }
        return Input.PixelsOn();
    }

    protected override Answer Part2()
    {
        return Input.Print();
    }

    protected override Screen Parse(string input)
    {
        var instructions = new List<Instruction>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var parts = line.Split(" ");
            if (parts[0] == "rect")
            {
                var dims = parts[1].Split("x");
                instructions.Add(new Rect() { Width = int.Parse(dims[0]), Height = int.Parse(dims[1]) });
            }
            else
            {
                var rowCol = int.Parse(parts[2].Split("=")[1]);
                var pixels = int.Parse(parts[4]);
                if (parts[1] == "row")
                {
                    instructions.Add(new RotateRow() { Row = rowCol, Pixels = pixels });
                }
                else // column
                {
                    instructions.Add(new RotateColumn() { Column = rowCol, Pixels = pixels });
                }
            }
        }

        return new Screen() { Instructions = instructions };
    }
}