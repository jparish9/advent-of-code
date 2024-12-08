using System.Data;

namespace AOC.AOC2024;

public class Day8 : Day<Day8.City>
{
    protected override string? SampleRawInput { get => "............\n........0...\n.....0......\n.......0....\n....0.......\n......A.....\n............\n............\n........A...\n.........A..\n............\n............"; }


    public class City
    {
        public City(char[][] map)
        {
            Antennas = new Dictionary<char, List<(int x, int y)>>();
            Width = map.Length;
            Height = map[0].Length;

            for (var x=0; x<Width; x++)
            {
                for (var y=0; y<Height; y++)
                {
                    if (map[x][y] != '.')
                    {
                        if (!Antennas.ContainsKey(map[x][y]))
                            Antennas[map[x][y]] = new List<(int x, int y)>();
                        Antennas[map[x][y]].Add((x, y));
                    }
                }
            }

        }
        public Dictionary<char, List<(int x, int y)>> Antennas;
        public int Width;
        public int Height;
    }

    private long CountAntinodes(bool all)
    {
        var antinodes = new List<(int x, int y)>();

        var allAntennas = Input.Antennas.Values.SelectMany(x => x).ToList();

        foreach (var antennaType in Input.Antennas.Keys)
        {
            var antennas = Input.Antennas[antennaType];

            for (var i=0; i<antennas.Count; i++)
            {
                for (var j=i+1; j<antennas.Count; j++)
                {
                    var (xd, yd) = (antennas[i].x - antennas[j].x, antennas[i].y - antennas[j].y);

                    // if "left", "right", start with "right" if counting all, otherwise 1 step past "right" (and only check one)
                    var check = (x: antennas[i].x - (all ? 1 : 2)*xd, y: antennas[i].y - (all ? 1 : 2)*yd);
                    while (check.x >= 0 && check.x < Input.Width && check.y >= 0 && check.y < Input.Height)
                    {
                        if (!antinodes.Contains(check))
                            antinodes.Add(check);
                        if (!all) break;
                        check = (x: check.x - xd, y: check.y - yd);
                    }

                    // repeat going the other direction starting with "right"
                    check = (x: antennas[j].x + (all ? 1 : 2)*xd, y: antennas[j].y + (all ? 1 : 2)*yd);
                    while (check.x >= 0 && check.x < Input.Width && check.y >= 0 && check.y < Input.Height)
                    {
                        if (!antinodes.Contains(check))
                            antinodes.Add(check);
                        if (!all) break;
                        check = (x: check.x + xd, y: check.y + yd);
                    }
                }
            }
        }

        return antinodes.Count;
    }

    protected override long Part1()
    {
        return CountAntinodes(false);
    }

    protected override long Part2()
    {
        return CountAntinodes(true);
    }

    protected override City Parse(string input)
    {
        return new City(input.Split("\n").Where(p => p != "").Select(p => p.Trim().ToCharArray()).ToArray());
    }
}