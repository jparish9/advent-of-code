using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AOC;

public abstract partial class Day<T>
{
    protected virtual string? SampleRawInput { get; }         // each Day must declare with get property
    protected virtual string? SampleRawInputPart2 { get; }    // only needed if different than SampleInput [part 1]; omit if same
    [AllowNull]
    protected T Input;                                         // set at runtime, usable by each Part implementation and never null
    protected bool IsPart2 {                                   // derived classes can reference this to determine if part 2 is being run
        get {
            // if IsPart2 is being checked from Parse, that means we need to store the parse result with a distinct cache key.
            if (!_part2ParsedDifferently && new StackFrame(1, false).GetMethod()!.Name.Contains("Parse"))
                _part2ParsedDifferently = true;

            return _isPart2;
        }
    }
    protected int InputHashCode => (RawInput + (_isPart2 && _part2ParsedDifferently)).GetHashCode();     // input hash, accounting for if part 2 needs to be parsed differently.  used by ParseInput, and can also be referenced by derived classes if further caching is possible.

    protected bool IsSampleInput => _useSampleInput;           // derived classes can reference this to determine if sample input is being used

    // typed parsing function to be implemented by each Day, taking the actual input or SampleInput[Part2] and returning the desired type.
    // not callable directly; called by ParseInput
    // Implementations can reference IsPart2 as needed if part 2 is to be parsed differently (also override Part2ParsedDifferently to true)
    protected abstract T Parse(string input);

    // algorithms to be implemented by each Day, returning each Part's single answer.
    // these use Input (either parsed sample or real input), and can also reference InputHashCode to do further caching if possible.
    // not callable directly; called by RunPart*
    protected abstract Answer Part1();
    protected abstract Answer Part2();


    // internal state
    private readonly Dictionary<int, T> _cache = new();
    private bool _isPart2 = false;
    private bool _useSampleInput = false;
    private string RawInput = "";
    private readonly Stopwatch _sw = new();
    private readonly int _year;
    private bool _part2ParsedDifferently = false;

    public Day()
    {
        if (!int.TryParse(GetType()?.Namespace?.Replace("AOC.AOC", ""), out int year))
            throw new Exception("Year cannot be determined, check namespace!");

        _year = year;
    }

    // public methods
    public Answer RunPart1(bool useSampleInput)
    {
        _isPart2 = false;
        _useSampleInput = useSampleInput;
        ConfigureInput();
        return Part1();
    }

    public Answer RunPart2(bool useSampleInput)
    {
        _isPart2 = true;
        _useSampleInput = useSampleInput;
        ConfigureInput();
        return Part2();
    }

    public void RunAll(bool realInputOnly = false)
    {
        if (!realInputOnly)
        {
            TryRun(() => RunPart1(true));
        }

        TryRun(() => RunPart1(false));

        if (!realInputOnly)
        {
            TryRun(() => RunPart2(true));
        }

        TryRun(() => RunPart2(false));
    }

    // this regex is used by several implementations to strip extra spaces from input
    [GeneratedRegex(" +")]
    protected static partial Regex Spaces();


    // remaining private methods for processing each Day; no more abstract or public methods below.
    private void ConfigureInput()
    {
        if (_useSampleInput)
        {
            // use static SampleInput from each implementation, error if not defined
            if (SampleRawInput == null)
                throw new Exception("SampleInput not defined!");

            if (SampleRawInputPart2 != null && _isPart2)
                RawInput = SampleRawInputPart2;
            else
                RawInput = SampleRawInput;
        }
        else
        {
            // load input from [year]/inputs/DayX.txt
            RawInput = File.ReadAllText($"{_year}/inputs/{GetType().Name}.txt");
        }

        Input = ParseInput();
    }


    private void TryRun(Func<Answer> func)
    {
        _sw.Start();
        try
        {
            var result = func();
            Console.WriteLine($"{_year} {GetType().Name} Part {(_isPart2 ? 2 : 1)} ({(_useSampleInput ? "sample" : "input")}): {result} in {_sw.ElapsedMilliseconds}ms");
        }
        catch (NotImplementedException)
        {
            Console.WriteLine($"{_year} {GetType().Name} Part {(_isPart2 ? 2 : 1)} ({(_useSampleInput ? "sample" : "input")}) not implemented yet");
        }
        catch (IOException)
        {
            Console.WriteLine($"{_year} {GetType().Name} Part {(_isPart2 ? 2 : 1)} ({(_useSampleInput ? "sample" : "input")}) input file not found");
        }
        _sw.Restart();
    }

    // call the derived Parse implementation to parse the input into the desired type, caching the result
    // if derived class has set Part2ParsedDifferently = true, save it with a different cache key.
    private T ParseInput()
    {
        var ih = InputHashCode;
        if (_cache.ContainsKey(ih)) return _cache[ih];

        var result = Parse(RawInput);

        _cache.Add(ih, result);
        return result;
    }


    // in some rare cases the answer is a string, but most of the time it is a number.
    // with the implicit operators, this is a little bit of syntactic sugar that allows for just returning a number or a string.
    public class Answer
    {
        public long Value { get; set; }
        public string? Text { get; set; }

        public Answer(long value, string? text = null)
        {
            Value = value;
            Text = text;
        }

        public static implicit operator Answer(long value) => new(value);
        public static implicit operator Answer(string text) => new(0, text);

        public override string ToString() => Text ?? Value.ToString();
    }
}
