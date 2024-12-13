namespace AOC.AOC2016;

public class Day9 : Day<Day9.File>
{
    protected override string? SampleRawInput { get => "(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN"; }

    public class File
    {
        public required string CompressedData;

        public long DecompressedSize(bool recursive)
        {
            return DecompressedSize(0, CompressedData.Length, recursive);
        }

        private long DecompressedSize(int start, int end, bool recursive)
        {
            var decompressedLength = 0L;
            var pos = start;
            var len = end;
            while (pos < len)
            {
                if (CompressedData[pos] == '(')         // found a marker
                {
                    var markerEnd = CompressedData.IndexOf(')', pos);       // assumes well-formed (closing ')' not after end pointer)
                    var marker = CompressedData.Substring(pos + 1, markerEnd - pos - 1).Split('x');
                    var chars = int.Parse(marker[0]);
                    var repeat = int.Parse(marker[1]);

                    decompressedLength += repeat *
                        (recursive ? DecompressedSize(markerEnd + 1, markerEnd + chars + 1, recursive)
                                    : chars);
                    pos = markerEnd + chars + 1;
                }
                else
                {
                    decompressedLength++;
                    pos++;
                }
            }

            return decompressedLength;
        }
    }

    protected override Answer Part1()
    {
        return Input.DecompressedSize(false);
    }

    protected override Answer Part2()
    {
        return Input.DecompressedSize(true);
    }

    protected override File Parse(string input)
    {
        return new File() { CompressedData = input.Replace("\n", "") };
    }
}