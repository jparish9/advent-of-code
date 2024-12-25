namespace AOC.AOC2024;

public class Day9 : Day<Day9.DiskMap>
{
    protected override string? SampleRawInput { get => "2333133121414131402"; }
    
    public class DiskMap
    {
        public required List<Block> Blocks;

        public long Checksum()
        {
            var sum = 0L;
            for (var i=0; i<Blocks.Count; i++)
            {
                sum += (Blocks[i].FileId ?? 0) * i;
            }
            return sum;
        }
    }

    public class Block
    {
        public int? FileId;             // null if free space
        public int FileSize;           // file size or free space size
    }

    public class FreeBlock
    {
        public int Start;
        public int Size;
    }

    protected override Answer Part1()
    {
        // make a copy so we don't have to parse it again, and we can modify the copy in-place (don't interfere with part 2)
        var map = new DiskMap() { Blocks = Input.Blocks.Select(p => new Block { FileId = p.FileId, FileSize = p.FileSize }).ToList() };

        var first = map.Blocks.FindIndex(p => p.FileId == null);
        var last = map.Blocks.FindLastIndex(p => p.FileId != null);
        while (first != -1 && last != -1 && first < last)
        {
            (map.Blocks[first], map.Blocks[last]) = (map.Blocks[last], map.Blocks[first]);          // inline swap

            while (first < map.Blocks.Count-1 && map.Blocks[++first].FileId != null);
            while (last > 0 && map.Blocks[--last].FileId == null);
        }

        return map.Checksum();
    }

    /*
    I had a number of issues not getting the right answer despite the sample input working fine.
    There are many edge cases which the sample input does not have, and even people with the same problem on the subreddit had different ones.
    I never figured out what the first edge case was in my implementation that looked a little like part 1 (walking files to the left, and within each walking free space to the right).
    After I saw a suggestion to use a sorted list/priority queue to keep track of the "best" free space, I tried that and ran into a different problem resulting from overcomplicating that sort.
    It turns out it only needs to be/stay sorted by position from left to right.  Now it runs in ~0.5s and gets the correct answer.
    */
    protected override Answer Part2()
    {
        // scan for all free blocks and put them in a list (sorted by position, because that is the order they are scanned/inserted in)
        var freeBlocks = new List<FreeBlock>();
        var i=0;
        while (i < Input.Blocks.Count)
        {
            if (Input.Blocks[i].FileId == null)
            {
                freeBlocks.Add(new FreeBlock { Start = i, Size = Input.Blocks[i].FileSize });
            }
            i += Input.Blocks[i].FileSize;
        }

        // start with the largest file id (rightmost) and work backwards to zero (left)
        var fileId = Input.Blocks.Max(p => p.FileId);
        var pos = Input.Blocks.FindLastIndex(p => p.FileId == fileId);

        while (fileId >= 0)
        {
            var size = Input.Blocks[pos].FileSize;

            var firstFit = freeBlocks.FirstOrDefault(p => p.Size >= size && p.Start < pos);         // leftmost free space block that fits this file
            if (firstFit == null)
            {
                // no valid free space found, move to the next file
                fileId--;
                while (fileId >= 0 && Input.Blocks[--pos].FileId != fileId);
                continue;
            }

            // swap firstFit and file ending at block pos
            for (var j=0; j<size; j++)
            {
                (Input.Blocks[firstFit.Start + j], Input.Blocks[pos - j]) = (Input.Blocks[pos - j], Input.Blocks[firstFit.Start + j]);          // inline swap
            }

            // if there is more free space in the firstFit free space block, reduce its FileSize and adjust its starting position, otherwise remove it
            if (firstFit.Size > size)
            {
                firstFit.Size -= size;
                firstFit.Start += size;
            }
            else
            {
                freeBlocks.Remove(firstFit);
            }

            // move to the next file
            fileId--;
            while (fileId >= 0 && Input.Blocks[--pos].FileId != fileId);
        }

       return Input.Checksum();
    }

    protected override DiskMap Parse(RawInput input)
    {
        var blocks = new List<Block>();
        var file = true;
        var fileId = 0;
        foreach (var ch in input.Value)
        {
            blocks.AddRange(Enumerable.Repeat(new Block() { FileId = file ? fileId : null, FileSize = ch - '0' }, ch - '0'));
            if (file) { fileId++; }
            file = !file;
        }

        return new DiskMap { Blocks = blocks };
    }
}