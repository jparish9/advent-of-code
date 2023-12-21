using System.Diagnostics.CodeAnalysis;

namespace AOC.AOC2023;

public class Day21 : Day<Day21.Garden>
{
    protected override string? SampleRawInput { get => "...........\n.....###.#.\n.###.##..#.\n..#.#...#..\n....#.#....\n.##..S####.\n.##..#...#.\n.......##..\n.##.#.####.\n.##..##.##.\n..........."; }

    public class Garden
    {
        public required char[][] Map { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }
        public int?[][] StepMap { get; set; }


        [SetsRequiredMembers]
        public Garden(char[][] map)
        {
            Map = map;
            XSize = map[0].Length;
            YSize = map.Length;

            StepMap = InitializeStepMap();

            var start = FindStart();
            StepMap[start.Item1][start.Item2] = 0;
            CalculateSteps(StepMap, new List<(int, int)> { start });
        }

        private int?[][] InitializeStepMap()
        {
            var stepMap = new int?[YSize][];
            for (var y=0; y<YSize; y++)
            {
                stepMap[y] = new int?[XSize];
                for (var x=0; x<XSize; x++)
                {
                    stepMap[y][x] = int.MaxValue;
                }
            }

            return stepMap;
        }

        private void CalculateSteps(int?[][] stepMap, List<(int, int)> positions)
        {
            while (positions.Count > 0)
            {
                var nextSteps = new List<(int, int)>();
                foreach (var step in positions)
                {
                    var nextStepsForThis = FindSteps(step, true);
                    foreach (var nextStep in nextStepsForThis)
                    {
                        if (stepMap[nextStep.Item1][nextStep.Item2] > stepMap[step.Item1][step.Item2] + 1)
                        {
                            stepMap[nextStep.Item1][nextStep.Item2] = stepMap[step.Item1][step.Item2] + 1;
                            nextSteps.Add(nextStep);
                        }
                    }
                }

                positions = nextSteps;
            }

            // null out unreachable
            for (var y=0; y<YSize; y++)
            {
                for (var x=0; x<XSize; x++)
                {
                    if (stepMap[y][x] == int.MaxValue)
                        stepMap[y][x] = null;
                }
            }
        }

        public (int, int) FindStart()
        {
            for (var y=0; y<Map.Length; y++)
            {
                for (var x=0; x<Map[y].Length; x++)
                {
                    if (Map[y][x] == 'S')
                    {
                        return (y, x);
                    }
                }
            }

            throw new Exception("No start found");
        }

        public List<(int, int)> FindSteps((int, int) pos, bool bounded)
        {
            var steps = new List<(int, int)>();

            // up
            if (pos.Item1 > 0 && Map[pos.Item1-1][pos.Item2] != '#')
                steps.Add((pos.Item1-1, pos.Item2));

            // down
            if (pos.Item1 < Map.Length-1 && Map[pos.Item1+1][pos.Item2] != '#')
                steps.Add((pos.Item1+1, pos.Item2));

            // left
            if (pos.Item2 > 0 && Map[pos.Item1][pos.Item2-1] != '#')
                steps.Add((pos.Item1, pos.Item2-1));

            // right
            if (pos.Item2 < Map[pos.Item1].Length-1 && Map[pos.Item1][pos.Item2+1] != '#')
                steps.Add((pos.Item1, pos.Item2+1));

            return steps;
        }
    }

    protected override long Part1()
    {
        var steps = Input.Map.Length > 11 ? 64 : 6;         // real or sample input

        return Input.StepMap.Sum(p => p.Count(q => q != null && q <= steps && q%2 == steps%2));
    }

    protected override long Part2()
    {
        if (Input.Map.Length <= 11) return 0;           // ignore sample for part 2.

        // all credit to https://github.com/villuna/aoc23/wiki/A-Geometric-solution-to-advent-of-code-2023,-day-21.

        // I would not have come to this solution on my own.  I suspected there was a stabilization that happened (obviously this couldn't be brute-forced),
        // but I was looking in the wrong place, trying to extend the grid in each direction until the step counts to keep traversing in that direction stabilized.
        // I saw that there was the same parity in each square, but because the real input is different I wouldn't have been able to see that the expansion results in a diamond shape,
        // or that the parity swaps between adjacent squares.

        // summarizing:
        // the actual input has properties the sample does not, which is annoying.
        // namely, in the actual input there is an empty row and column going through our S.
        // the input is 131x131, which means the requested number of steps is specially chosen.
        // 26501365/131 = 202300, and 26501365 % 131 = 65.
        // thus if we go in the same direction (any of the 4 directions), we traverse exactly 202300 grid extensions (all the way to the edge of that final grid),
        // beginning with 65 steps to get to the edge of our home grid.
        // because of this symmetry, the maximum extent draws out a diamond shape.
        // in part 1, I noticed that each tile has parity, that is, any given tile can only be reached in an odd or an even number of steps (not both).
        // we will also need the minimum step counts from the search done in part 1, both for parity and to exclude "corners" at the border (see below).
        // as 131 is odd, this means that each square has the opposite parity of its adajacent squares.
        // if we start with odd parity at the center, it means that for our diamond size of 202300 (even) in any direction, the border squares will also have odd parity.
        // the total number of squares at each expansion of the diamond (if we increase n by 2 in steps) is (n+1)^2 odd and n^2 even.
        // that leaves the corners at the edge of our diamond.
        // we need to subtract out outer corners of odd squares and add an extra even corner from even squares to complete our diamond
        // (given that odd squares are the partial squares on the border of our diamond).

        // note that none of this works if our requested steps results in (steps - MapSize/2)/MapSize not being an integer (or the map not being square),
        // and it would have to be adjusted for parity if n came out odd.
        var evenCorners = (long)Input.StepMap.Sum(p => p.Count(q => q != null && q % 2 == 0 && q > 65));
        var oddCorners = (long)Input.StepMap.Sum(p => p.Count(q => q != null && q % 2 == 1 && q > 65));

        var evenFull = (long)Input.StepMap.Sum(p => p.Count(q => q != null && q % 2 == 0));
        var oddFull = (long)Input.StepMap.Sum(p => p.Count(q => q != null && q % 2 == 1));

        var n = (26501365L - (Input.Map.Length/2))/Input.Map.Length;

        return (n+1)*(n+1)*oddFull + n*n*evenFull - (n+1)*oddCorners + n*evenCorners;
    }

    protected override Garden Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();
        var map = new char[lines.Length][];

        for (var y=0; y<lines.Length; y++)
        {
            map[y] = lines[y].ToCharArray();
        }

        return new Garden(map);
    }
}