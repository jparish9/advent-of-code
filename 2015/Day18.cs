using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using AOC.Utils;

namespace AOC.AOC2015;

public class Day18 : Day<Day18.Lights>
{
    protected override string? SampleRawInput { get => ".#.#.#\n...##.\n#....#\n..#...\n#.#..#\n####.."; }

    public class Lights
    {
        public required char[][] Grid { get; set; }

        private bool _forceCornersOn = false;

        // part 2
        public void ForceCornersOn()
        {
            _forceCornersOn = true;
            Grid[0][0] = '#';
            Grid[0][^1] = '#';
            Grid[^1][0] = '#';
            Grid[^1][^1] = '#';
        }
        
        public Lights Copy()
        {
            return new Lights() { Grid = NewGrid((x, y) => Grid[y][x]) };
        }

        public char[][] NewGrid()
        {
            return NewGrid((x, y) => '.');
        }

        private char[][] NewGrid(Func<int, int, char> gridChar)
        {
            var newGrid = new char[Grid.Length][];

            for (var y=0; y<Grid.Length; y++)
            {
                newGrid[y] = new char[Grid[y].Length];
                for (var x=0; x<Grid[y].Length; x++)
                {
                    newGrid[y][x] = gridChar(x, y);
                }
            }

            return newGrid;
        }

        public void Next(int steps)
        {
            for (var i=0; i<steps; i++)
            {
                var newLights = NewGrid();

                for (var y=0; y<Grid.Length; y++)
                {
                    for (var x=0; x<Grid[y].Length; x++)
                    {
                        if (_forceCornersOn && (x == 0 || x == Grid[y].Length-1) && (y == 0 || y == Grid.Length-1))         // part 2
                        {
                            newLights[y][x] = '#';
                            continue;
                        }

                        // count neighbors "on"
                        var count = 0;
                        for (var y2=y-1; y2<=y+1; y2++)
                        {
                            for (var x2=x-1; x2<=x+1; x2++)
                            {
                                if (y2 < 0 || y2 >= Grid.Length || x2 < 0 || x2 >= Grid[y2].Length || (x2 == x && y2 == y)) continue;
                                if (Grid[y2][x2] == '#') count++;
                            }
                        }

                        if (Grid[y][x] == '#') newLights[y][x] = count == 2 || count == 3 ? '#' : '.';          // if on, stay on if exactly 2 or 3 neighbors are on
                        else newLights[y][x] = count == 3 ? '#' : '.';                                          // if off, turn on if exactly 3 neighbors are on
                    }
                }

                Grid = newLights;
            }
        }
    }

    protected override Answer Part1()
    {
        return Run(false);
    }

    protected override Answer Part2()
    {
        return Run(true);
    }

    private long Run(bool forceCornersOn)
    {
        var lights = Input.Copy();
        if (forceCornersOn) lights.ForceCornersOn();

        lights.Next(lights.Grid.Length == 6 ? (IsPart2 ? 5 : 4) : 100);         // sample or real input

        return lights.Grid.Sum(p => p.Count(q => q == '#'));
    }

    protected override Lights Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();
        var grid = new char[lines.Length][];

        for (var y=0; y<lines.Length; y++)
        {
            grid[y] = lines[y].ToCharArray();
        }

        return new Lights() { Grid = grid };
    }
}