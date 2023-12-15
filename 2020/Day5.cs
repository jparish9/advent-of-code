namespace AOC.AOC2020;

public class Day5 : Day<List<Day5.Seat>>
{
    protected override string? SampleRawInput { get => "FBFBBFFRLR\nBFFFBBFRRR\nFFFBBBFRRR\nBBFFBBFRLL"; }

    public class Seat
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Id { get; set; }
    }

    protected override long Part1()
    {
        return Input.Max(p => p.Id);
    }

    protected override long Part2()
    {
        var seats = Input.OrderBy(p => p.Id).ToList();
        for (var i=0; i<seats.Count-1; i++)
        {
            if (seats[i].Id + 1 != seats[i+1].Id)
            {
                return seats[i].Id + 1;
            }
        }

        return -1;
    }

    protected override List<Seat> Parse(string input)
    {
        var seats = input.Split('\n').Where(p => p != "").Select(p => {
            return new Seat {
                Row = Convert.ToInt32(p[..7].Replace('F', '0').Replace('B', '1'), 2),
                Col = Convert.ToInt32(p[7..].Replace('L', '0').Replace('R', '1'), 2)
            };
        }).ToList();

        foreach (var seat in seats)
        {
            seat.Id = seat.Row * 8 + seat.Col;
        }

        return seats;
    }
}