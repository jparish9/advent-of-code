using System.Data;
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
            StepMap[start.Y][start.X] = 0;
            CalculateSteps(StepMap, new List<(int, int)> { start });
        }

        // return a new Garden with the given map repeated factor times in each direction from the center
        public static Garden Expand(char[][] map, int factor)
        {
            var f = 1+factor*2;
            char[][] expanded = new char[map.Length * f][];

            for (var i=0; i<expanded.Length; i++)
            {
                expanded[i] = new char[map[0].Length * f];
            }

            for (var i=0; i<f; i++)
            {
                for (var j=0; j<f; j++)
                {
                    for (var y=0; y<map.Length; y++)
                    {
                        for (var x=0; x<map[y].Length; x++)
                        {
                            expanded[i*map.Length+y][j*map[y].Length+x] = map[y][x] == 'S' ? (i==factor && j==factor ? 'S' : '.') : map[y][x];
                        }
                    }
                }
            }

            return new Garden(expanded);
        }

        public long EvalSteps(int steps)
        {
            return StepMap.Sum(p => p.Count(q => q != null && q <= steps && q%2 == steps%2));
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

        private void CalculateSteps(int?[][] stepMap, List<(int Y, int X)> positions)
        {
            while (positions.Count > 0)
            {
                var nextSteps = new List<(int, int)>();
                foreach (var step in positions)
                {
                    var nextStepsForThis = FindSteps(step);
                    foreach (var nextStep in nextStepsForThis)
                    {
                        if (stepMap[nextStep.Y][nextStep.X] > stepMap[step.Y][step.X] + 1)
                        {
                            stepMap[nextStep.Y][nextStep.X] = stepMap[step.Y][step.X] + 1;
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

        public (int Y, int X) FindStart()
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

        public List<(int Y, int X)> FindSteps((int Y, int X) pos)
        {
            var steps = new List<(int, int)>();

            // up
            if (pos.Y > 0 && Map[pos.Y-1][pos.X] != '#')
                steps.Add((pos.Y-1, pos.X));

            // down
            if (pos.Y < Map.Length-1 && Map[pos.Y+1][pos.X] != '#')
                steps.Add((pos.Y+1, pos.X));

            // left
            if (pos.X > 0 && Map[pos.Y][pos.X-1] != '#')
                steps.Add((pos.Y, pos.X-1));

            // right
            if (pos.X < Map[pos.Y].Length-1 && Map[pos.Y][pos.X+1] != '#')
                steps.Add((pos.Y, pos.X+1));

            return steps;
        }
    }

    protected override Answer Part1()
    {
        var steps = Input.Map.Length > 11 ? 64 : 6;         // real or sample input

        return Input.EvalSteps(steps);
    }

    protected override Answer Part2()
    {
        if (Input.Map.Length <= 11) return 0;           // ignore sample for part 2.

        // because of the properties of our actual input (not the sample), we get to the edge of the home grid in (map size/2) steps.  (there is a clear path from our S in each direction)
        // as we are expanding in two dimensions, we can try fitting a quadratic to the first 3 numbers of steps that we expect to be cyclic,
        // that is, (map size/2), (map size/2 + map size), and (map size/2 + 2*map size).
        // we can quickly calculate these first 3 points, then evaluate at the desired step count,
        // which is 26501365 (not an arbitrary number, 26501365 % 131 = 65, so we get to the edge of 26501365 / 131 (integer division) complete grids in each direction plus our starting grid.

        var s = Input.Map.Length;
        var x0 = s / 2;
        var x = new List<long> { 0, 1, 2 };
        var y = new List<long> {
            Input.EvalSteps(x0),
            Garden.Expand(Input.Map, 1).EvalSteps(x0 + s),
            Garden.Expand(Input.Map, 2).EvalSteps(x0 + 2*s)
        };

        // fit a quadratic, with x=0,1,2 this is simplified from the general formula

        // y[0] = a*0^2 + b*0 + c  ==> c = y[0]
        // y[1] = a*1^2 + b*1 + c  ==> a + b = y[1] - y[0]
        // y[2] = a*2^2 + b*2 + c  ==> 4a + 2b = y[2] - y[0]
        // 2a = y[2] - 2(y[1] - y[0]) ==> a = (y[2] - 2*y[1] + y[0]) / 2
        // b = y[1] - y[0] - a

        var c = y[0];
        var a = (y[2] - 2*y[1] + y[0]) / 2;
        var b = y[1] - y[0] - a;

        // check x=3
        var y3 = Garden.Expand(Input.Map, 3).EvalSteps(x0 + 3*s);
        if (y3 != a*3*3 + b*3 + c)
            throw new Exception("Quadratic fit failed!");

        var n = 26501365 / Input.Map.Length;

        return a*n*n + b*n + c;
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