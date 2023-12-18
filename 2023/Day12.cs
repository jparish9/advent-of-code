namespace AOC.AOC2023;

public class Day12 : Day<Day12.SpringRowGroup>
{
    protected override string? SampleRawInput { get => "???.### 1,1,3\n.??..??...?##. 1,1,3\n?#?#?#?#?#?#?#? 1,3,1,6\n????.#...#... 4,1,1\n????.######..#####. 1,6,5\n?###???????? 3,2,1"; }
    protected override bool Part2ParsedDifferently => true;

    public class SpringRowGroup
    {
        public required List<SpringRow> Rows { get; set; }

        public long GetTotalPermutations()
        {
            return Rows.Sum(p => p.GetPermutations());
        }
    }

    public class SpringRow
    {
        public required string Row { get; set; }
        public required int[] DamagedGroups { get; set; }
        private readonly Dictionary<(int, int, int), long> _memoized = new();

        public long GetPermutations()
        {
            _memoized.Clear();
            return GetPermutations(Row, DamagedGroups, 0, 0, 0);
        }

        // okay, this one I thought about for most of the morning on my own but didn't really get anywhere.
        // I thought of it as a combinatorics problem, but couldn't figure out how to reduce it to such (like repeated applications of nCr or something).
        // I looked at the subreddit and found a hint that it was a dynamic programming/tail recursion problem, and then I found this pretty concise solution.
        // Not much of this implementation is my own idea (it's basically https://github.com/jonathanpaulson/AdventOfCode/blob/master/2023/12.py in C#),
        // but I did learn a lot and added my own comments for understanding.
        // It is pretty cool that for a given row, the number of permutations is completely determined by the current row position, the current group position, and the current accumulated group length.

        // memoization is for part 2, since each row is repeated 5x there is a lot of overlap in the recursion tree
        private long GetPermutations(string row, int[] groups, int rowPos, int groupPos, int currentLength)
        {
            if (_memoized.ContainsKey((rowPos, groupPos, currentLength))) return _memoized[(rowPos, groupPos, currentLength)];

            if (rowPos >= row.Length)
            {
                if (groupPos == groups.Length && currentLength == 0) return 1;                          // end of string and groups, nothing to do, valid permutation
                else if (groupPos == groups.Length-1 && groups[groupPos] == currentLength) return 1;    // last character of string completes last group, valid permutation
                else return 0;                                                                          // otherwise invalid permutation (reached the end of row with incomplete group(s))
            }

            var permutations = 0L;
            foreach (var possible in new char[] { '.', '#'})
            {
                if (row[rowPos] == possible || row[rowPos] == '?')                                          // test with matching character or '?' (wildcard)
                {
                    if (possible == '.' && currentLength == 0)
                        permutations += GetPermutations(row, groups, rowPos+1, groupPos, 0);                   // not in a group, empty space, just go to next character
                    else if (possible == '.' && currentLength > 0 && groupPos < groups.Length && groups[groupPos] == currentLength)
                        permutations += GetPermutations(row, groups, rowPos+1, groupPos+1, 0);                 // in a group, current character completes the group, go to next character and group
                    else if (possible == '#')
                        permutations += GetPermutations(row, groups, rowPos+1, groupPos, currentLength+1);     // in/starting a group, current character is part of the group, go to next character
                }
            }

            _memoized.Add((rowPos, groupPos, currentLength), permutations);
            return permutations;
        }
    }

    protected override long Part1()
    {
        return Input.GetTotalPermutations();
    }

    protected override long Part2()
    {
        return Input.GetTotalPermutations();
    }

    protected override SpringRowGroup Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToArray();

        var result = new List<SpringRow>();

        foreach (var line in lines)
        {
            var row = line.Split(' ')[0];
            var groups = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();

            var finalRow = row;
            var finalGroups = groups.ToList();

            if (IsPart2)
            {
                // for part 2, each row is repeated 5x (with a "?" separator)
                for (var i=0; i<4; i++)
                {
                    finalRow += "?" + row;
                    finalGroups.AddRange(groups);
                }
            }

            result.Add(new SpringRow() { Row = finalRow, DamagedGroups = finalGroups.ToArray() });
        }

        return new SpringRowGroup() { Rows = result };
    }
}