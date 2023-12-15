namespace AOC.AOC2023;

public class Day9 : Day<List<List<int>>>
{
    protected override string? SampleRawInput { get => "0 3 6 9 12 15\n1 3 6 10 15 21\n10 13 16 21 30 45"; }

    private readonly Dictionary<int, int[]> _predictions = new();

    protected override long Part1()
    {
        return GetPrediction()[1];
    }

    protected override long Part2()
    {
        return GetPrediction()[0];
    }

    // get forward and backward predictions (both parts) in one loop, cache the results
    private int[] GetPrediction()
    {
        var ih = InputHashCode;
        if (_predictions.ContainsKey(ih)) return _predictions[ih];

        var preds = new int[2];

        var backwardPred = 0;
        var forwardPred = 0;

        foreach (var line in Input)
        {
            var diffs = new List<List<int>>() { line };

            while (diffs[^1].Any(p => p != 0))
            {
                diffs.Add(new List<int>());
                for (var i=0; i<diffs[^2].Count-1; i++)
                {
                    diffs[^1].Add(diffs[^2][i+1] - diffs[^2][i]);
                }
            }

            var last = diffs[^2][^1];
            var first = diffs[^2][0];

            for (var i=diffs.Count-3; i>=0; i--)
            {
                last += diffs[i][^1];
                first = diffs[i][0] - first;
            }

            forwardPred += last;
            backwardPred += first;
        }

        _predictions.Add(ih, new int[2] { backwardPred, forwardPred });
        return _predictions[ih];
    }

    protected override List<List<int>> Parse(string input)
    {
        var lines = input.Split('\n').Where(p => p != "").ToList();
        var parsed = new List<List<int>>();
        foreach(var line in lines)
        {
            parsed.Add(line.Split(' ').Select(int.Parse).ToList());
        }

        return parsed;
    }
}