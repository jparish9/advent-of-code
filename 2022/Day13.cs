using System.Text;

namespace AOC.AOC2022;

public class Day13 : Day<List<Day13.Packet>>
{
    protected override string? SampleRawInput { get => "[1,1,3,1,1]\n[1,1,5,1,1]\n\n[[1],[2,3,4]]\n[[1],4]\n\n[9]\n[[8,7,6]]\n\n[[4,4],4,4]\n[[4,4],4,4,4]\n\n[7,7,7,7]\n[7,7,7]\n\n[]\n[3]\n\n[[[]]]\n[[]]\n\n[1,[2,[3,[4,[5,6,7]]]],8,9]\n[1,[2,[3,[4,[5,6,0]]]],8,9]"; }

    public class Packet : IComparable<Packet>
    {
        public required List<PacketItem> Items { get; set; }

        public int CompareTo(Packet? other)
        {
            if (other == null) return 1;

            var ct = Math.Max(Items.Count, other.Items.Count);

            for (var i=0; i<ct; i++)
            {
                if (i >= Items.Count) return -1;            // this packet ran out of items
                if (i >= other.Items.Count) return 1;       // other packet ran out of items

                var first = Items[i];
                var second = other.Items[i];

                if (first.Number != null && second.Number != null)                      // both are numbers
                {
                    var result = first.Number.Value.CompareTo(second.Number.Value);
                    if (result != 0) return result;
                }
                else if (first.SubPacket != null && second.SubPacket != null)           // both are sub-packets
                {
                    var result = first.SubPacket.CompareTo(second.SubPacket);
                    if (result != 0) return result;
                }
                // one is a number, one is a sub-packet; compare as if the number is a sub-packet containing a single number
                else if (first.Number != null && second.SubPacket != null)
                {
                    var result = new Packet() { Items = new List<PacketItem>() { new() { Number = first.Number } } }.CompareTo(second.SubPacket);
                    if (result != 0) return result;
                }
                else if (first.SubPacket != null && second.Number != null)
                {
                    var result = first.SubPacket.CompareTo(new Packet() { Items = new List<PacketItem>() { new() { Number = second.Number } } });
                    if (result != 0) return result;
                }
            }

            return 0;
        }
    }

    // each packet item is either a number or a sub-packet
    public class PacketItem
    {
        public long? Number { get; set; }
        public Packet? SubPacket { get; set; }
    }

    protected override Answer Part1()
    {
        var sum = 0;
        for (var i=0; i<Input.Count; i+=2)
        {
            if (Input[i].CompareTo(Input[i+1]) < 0) sum += i/2+1;
        }
        return sum;
    }

    protected override Answer Part2()
    {
        var all = new List<Packet>();
        all.AddRange(Input);

        // add dividers
        var divider2 = ParsePacket("[[2]]");
        var divider6 = ParsePacket("[[6]]");

        all.Add(divider2);
        all.Add(divider6);

        // sort the whole list
        all.Sort();

        return (all.IndexOf(divider2)+1) * (all.IndexOf(divider6)+1);
    }

    public static string PrintPacket(Packet packet)
    {
        var sb = new StringBuilder();
        sb.Append('[');

        for (var i=0; i<packet.Items.Count; i++)
        {
            sb.Append(PrintPacketItem(packet.Items[i]));
            if (i < packet.Items.Count - 1) sb.Append(',');
        }

        sb.Append(']');
        return sb.ToString();
    }

    public static string PrintPacketItem(PacketItem item)
    {
        var sb = new StringBuilder();
        if (item.Number != null)
        {
            sb.Append(item.Number);
        }
        else
        {
            sb.Append(PrintPacket(item.SubPacket!));
        }
        return sb.ToString();
    }

    protected override List<Packet> Parse(RawInput input)
    {
        var packets = new List<Packet>();

        foreach (var pair in input.LineGroups())
        {
            packets.Add(ParsePacket(pair[0]));
            packets.Add(ParsePacket(pair[1]));
        }

        return packets;
    }

    private Packet ParsePacket(string packetString)
    {
        var packetItems = new List<PacketItem>();

        // remove outer brackets
        packetString = packetString[1..^1];

        // split on commas, but only if not inside brackets
        var items = new List<string>();
        var current = "";
        var bracketCount = 0;
        foreach (var c in packetString)
        {
            if (c == '[') bracketCount++;
            if (c == ']') bracketCount--;
            if (c == ',' && bracketCount == 0)
            {
                items.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        items.Add(current);

        foreach (var item in items)
        {
            if (item.StartsWith("[")) packetItems.Add(new PacketItem() { SubPacket = ParsePacket(item) });          // sub-packet
            else if (item != "") packetItems.Add(new PacketItem() { Number = long.Parse(item) });                   // single number
        }

        return new Packet() { Items = packetItems };
    }
}