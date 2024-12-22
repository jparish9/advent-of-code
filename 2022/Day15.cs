using System.Text.RegularExpressions;

namespace AOC.AOC2022;

public partial class Day15 : Day<Day15.Tunnels>
{
    protected override string? SampleRawInput { get => "Sensor at x=2, y=18: closest beacon is at x=-2, y=15\nSensor at x=9, y=16: closest beacon is at x=10, y=16\nSensor at x=13, y=2: closest beacon is at x=15, y=3\nSensor at x=12, y=14: closest beacon is at x=10, y=16\nSensor at x=10, y=20: closest beacon is at x=10, y=16\nSensor at x=14, y=17: closest beacon is at x=10, y=16\nSensor at x=8, y=7: closest beacon is at x=2, y=10\nSensor at x=2, y=0: closest beacon is at x=2, y=10\nSensor at x=0, y=11: closest beacon is at x=2, y=10\nSensor at x=20, y=14: closest beacon is at x=25, y=17\nSensor at x=17, y=20: closest beacon is at x=21, y=22\nSensor at x=16, y=7: closest beacon is at x=15, y=3\nSensor at x=14, y=3: closest beacon is at x=15, y=3\nSensor at x=20, y=1: closest beacon is at x=15, y=3"; }

    public class Tunnels
    {
        public required List<Sensor> Sensors;
        public required List<(int X, int Y)> BeaconsInRange;
    }

    public class Sensor
    {
        public (int X, int Y) Position;
        public int Range;                   // manhattan distance

        public (int Low, int High)? CoverageAtRow(int row)
        {
            var y = Position.Y;
            var x = Position.X;
            var range = Range;

            if (row < y - range || row > y + range) return null;

            var low = x - (range - Math.Abs(row - y));
            var high = x + (range - Math.Abs(row - y));

            return (low, high);
        }
    }

    protected override Answer Part1()
    {
        var coverage = new HashSet<int>();
        var row = IsSampleInput ? 10 : 2000000;

        foreach (var sensor in Input.Sensors)
        {
            var coverageAtRow = sensor.CoverageAtRow(row);
            if (coverageAtRow != null)
            {
                for (var x = coverageAtRow.Value.Low; x <= coverageAtRow.Value.High; x++)
                {
                    coverage.Add(x);
                }
            }
        }
        return coverage.Count - Input.BeaconsInRange.Count(p => p.Y == row);
    }

    protected override Answer Part2()
    {
        var (extentX, extentY) = (IsSampleInput ? 20 : 4000000, IsSampleInput ? 20 : 4000000);
        var (posX, posY) = (0, 0);

        while (true)
        {
            // find the first sensor in range
            var sensor = Input.Sensors.FirstOrDefault(p => Math.Abs(p.Position.Y - posY) + Math.Abs(p.Position.X - posX) <= p.Range);
            if (sensor == null) break;          // none in range, found the hidden beacon

            // skip forward to outside this sensor's range on this row; go to next row if outside the extent
            posX = sensor.Position.X - Math.Abs(sensor.Position.Y - posY) + sensor.Range + 1;
            if (posX > extentX)
            {
                posX = 0;
                posY++;
            }
            if (posY > extentY) throw new Exception("Not found");
        }

        return (long)posX * 4000000 + posY;
    }

    protected override Tunnels Parse(string input)
    {
        var sensors = new List<Sensor>();
        var beaconsInRange = new HashSet<(int X, int Y)>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var match = SensorRegex().Matches(line);
            
            var groups = match[0].Groups;

            var (x, y) = (int.Parse(groups[1].Value), int.Parse(groups[2].Value));
            var (beaconX, beaconY) = (int.Parse(groups[3].Value), int.Parse(groups[4].Value));

            var range = Math.Abs(x - beaconX) + Math.Abs(y - beaconY);

            beaconsInRange.Add((beaconX, beaconY));

            sensors.Add(new Sensor() { Position = (x, y), Range = range });
        }

        return new Tunnels() { Sensors = sensors, BeaconsInRange = [.. beaconsInRange] };
    }

    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex SensorRegex();
 
}