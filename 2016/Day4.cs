using System.Text.RegularExpressions;

namespace AOC.AOC2016;

public class Day4 : Day<Day4.Kiosk>
{
    protected override string? SampleRawInput { get => "aaaaa-bbb-z-y-x-123[abxyz]\na-b-c-d-e-f-g-h-987[abcde]\nnot-a-real-room-404[oarel]\ntotally-real-room-200[decoy]"; }

    public class Kiosk
    {
        public required List<Room> Rooms { get; set; }
    }

    public class Room
    {
        public required string Name { get; set; }
        public int SectorId { get; set; }
        public required string Checksum { get; set; }
    }

    protected override Answer Part1()
    {
        var valid = 0;
        foreach (var room in Input.Rooms)
        {
            var checksum = new string(room.Name.Replace("-", "").GroupBy(p => p).OrderByDescending(p => p.Count()).ThenBy(p => p.Key).Take(5).Select(p => p.Key).ToArray());

            if (checksum == room.Checksum) valid += room.SectorId;
        }

        return valid;
    }

    protected override Answer Part2()
    {
        foreach (var room in Input.Rooms)
        {
            var decrypted = new string(room.Name.Select(p => p == '-' ? ' ' : (char)(((p - 'a' + room.SectorId) % 26) + 'a')).ToArray());
            if (decrypted == "northpole object storage") return room.SectorId;
        }

        return 0;
    }

    protected override Kiosk Parse(RawInput input)
    {
        var rooms = new List<Room>();

        foreach (var line in input.Lines())
        {
            var match = Regex.Match(line, @"([a-z\-]+)-(\d+)\[([a-z]+)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            rooms.Add(new Room() { Name = match.Groups[1].Value, SectorId = int.Parse(match.Groups[2].Value), Checksum = match.Groups[3].Value });
        }

        return new Kiosk() { Rooms = rooms };

    }
}