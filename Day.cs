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
    protected virtual bool Part2ParsedDifferently => false; // derived classes should override this to true if part 2 input needs to be parsed differently than part 1
    protected bool IsPart2 => _isPart2;                     // read-only property for derived classes to access in Parse implementations
    protected int InputHashCode => (RawInput + (_isPart2 && Part2ParsedDifferently)).GetHashCode();     // input hash, accounting for if part 2 needs to be parsed differently.  used by ParseInput, and can also be referenced by derived classes if further caching is possible.


    // typed parsing function to be implemented by each Day, taking the actual input or SampleInput[Part2] and returning the desired type.
    // not callable directly; called by ParseInput
    // Implementations can reference IsPart2 as needed if part 2 is to be parsed differently (also override Part2ParsedDifferently to true)
    protected abstract T Parse(string input);

    // algorithms to be implemented by each Day, returning each Part's single answer.
    // these use Input (either parsed sample or real input), and can also reference InputHashCode to do further caching if possible.
    // not callable directly; called by RunPart*
    protected abstract long Part1();
    protected abstract long Part2();


    // internal state
    private readonly Dictionary<int, T> _cache = new();
    private bool _isPart2 = false;
    private bool _useSampleInput = false;
    private string RawInput = "";
    private readonly Stopwatch _sw = new();
    private readonly int _year;

    public Day()
    {
        if (!int.TryParse(GetType()?.Namespace?.Replace("AOC.AOC", ""), out int year))
            throw new Exception("Year cannot be determined, check namespace!");

        _year = year;
    }

    // public methods
    public long RunPart1(bool useSampleInput)
    {
        _isPart2 = false;
        _useSampleInput = useSampleInput;
        ConfigureInput();
        return Part1();
    }

    public long RunPart2(bool useSampleInput)
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


    private void TryRun(Func<long> func)
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
        catch (FileNotFoundException)
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
}
