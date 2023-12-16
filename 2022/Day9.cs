namespace AOC.AOC2022;

public class Day9 : Day<List<Day9.Move>>
{
    protected override string? SampleRawInput { get => "R 4\nU 4\nL 3\nD 1\nR 4\nD 1\nL 5\nR 2"; }
    //protected override string? SampleRawInput { get => "R 5\nU 8\nL 8\nD 3\nR 17\nD 10\nL 25\nU 20"; }

    public class Move
    {
        public char Direction { get; set; }
        public int Distance { get; set; }
    }

    private class Rope
    {
        public Rope(int numTails)
        {
            Head = new Coord() { X = 0, Y = 0 };
            Tails = new List<Coord>();
            for (var i=0; i<numTails; i++)
            {
                Tails.Add(new Coord() { X = 0, Y = 0 });
            }
        }

        public Coord Head { get; set; }
        public List<Coord> Tails { get; set; }

        public HashSet<(int, int)> TailPositions { get; set; } = new HashSet<(int, int)>() { (0, 0) };

        public void ApplyMove(Move move)
        {
            for (var i=1; i<=move.Distance; i++)
            {
                Head.X += move.Direction == 'R' ? 1 : (move.Direction == 'L' ? -1 : 0);
                Head.Y += move.Direction == 'U' ? 1 : (move.Direction == 'D' ? -1 : 0);

                // adjust each tail position in sequence.  the current tail piece's new position is only dependent on the previous piece's new position.
                var followX = Head.X;
                var followY = Head.Y;

                foreach (var tail in Tails)
                {
                    var xDist = followX - tail.X;
                    var yDist = followY - tail.Y;
                    if (Math.Abs(xDist) > 1)            // this tail piece is 2,[0-2] away from the previous.  move it to 1,0-1
                    {
                        tail.X += xDist > 0 ? 1 : -1;
                        if (followY != tail.Y) tail.Y += yDist > 0 ? 1 : -1;
                    }
                    else if (Math.Abs(yDist) > 1)        // this tail piece is [0-1],2 away from the previous.  move it to 0,1
                    {
                        tail.Y += yDist > 0 ? 1 : -1;
                        if (followX != tail.X) tail.X += xDist > 0 ? 1 : -1;
                    }

                    followX = tail.X;
                    followY = tail.Y;
                }

                var lastTail = (Tails[^1].X, Tails[^1].Y);

                if (!TailPositions.Contains(lastTail))
                {
                    TailPositions.Add(lastTail);
                }

            }
        }
    }

    private class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    protected override long Part1()
    {
        return RopeBridge(1);
    }

    protected override long Part2()
    {
        return RopeBridge(9);
    }

    private long RopeBridge(int tails)
    {
        var rope = new Rope(tails);
        Input.ForEach(rope.ApplyMove);

        return rope.TailPositions.Count;
    }

    protected override List<Move> Parse(string input)
    {
        var moves = new List<Move>();
        foreach (var line in input.Split('\n').Where(p => p != ""))
        {
            var move = new Move() { Direction = line[0], Distance = int.Parse(line[2..]) };
            moves.Add(move);
        }

        return moves;
    }
}