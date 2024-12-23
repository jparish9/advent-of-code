namespace AOC.AOC2017;

public class Day2 : Day<Day2.Spreadsheet>
{
    protected override string? SampleRawInput { get => "5\t1\t9\t5\n7\t5\t3\n2\t4\t6\t8"; }
    protected override string? SampleRawInputPart2 { get => "5\t9\t2\t8\n9\t4\t7\t3\n3\t8\t6\t5"; }

    public class Spreadsheet
    {
        public required List<List<int>> Rows;
    }

    protected override Answer Part1()
    {
        return Input.Rows.Select(p => p.Max() - p.Min()).Sum();
    }

    protected override Answer Part2()
    {
        return Input.Rows.Select(p => p.SelectMany(q => p.Where(r => r != q && r % q == 0).Select(r => r / q)).First()).Sum();
    }

    protected override Spreadsheet Parse(string input)
    {
        return new Spreadsheet() { Rows = input.Split("\n").Where(p => p != "").Select(p => p.Split("\t").Select(q => int.Parse(q)).ToList()).ToList() };
    }
}