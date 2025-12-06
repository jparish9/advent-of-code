namespace AOC;

using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        // optional day/year as args.
        // if year not provided, run current year
        // if day not provided, run all days for the year and report total time
        // if specific day provided, run just that day (for the provided/current year)
        bool sampleOnly = false;
        int? day = null;
        int year = DateTime.Now.Year;
        if (args.Length > 0)
        {
            if (args.Contains("--sampleonly"))
            {
                sampleOnly = true;
                args = [.. args.Where(a => a != "--sampleonly")];
            }

            var second = -1;
            if (!int.TryParse(args[0], out int first)
                || (args.Length > 1 && !int.TryParse(args[1], out second)))
            {
                PrintUsage();
                return;
            }

            day = first > 25 ? (second != -1 ? second : null) : first;
            year = second > 25 ? second : (first > 25 ? first : year);

            if (day > 25) {
                PrintUsage();
                return;
            }
        }

        // find all Day implementations according to the parameters given
        var baseType = typeof(Day<>);
        var all = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface
                && (t.Namespace?.EndsWith(year.ToString()) ?? false)
                && t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Day<>)
                && int.TryParse(t.Name.Replace("Day", ""), out int tmp)
                && (day == null || t.Name == $"Day{day}")));

        // if a specific day was provided and is not yet implemented, scaffold the class (from DayTemplate)
        if (day != null && !all.Any())
        {
            Console.WriteLine($"Day{day} implementation not found for {year}... scaffolding it");
            ScaffoldClass(year, day.Value);
            return;
        }

        // run day(s)
        var sw2 = new Stopwatch();
        sw2.Start();
        foreach (var cls in all.OrderBy(p => int.Parse(p.Name.Replace("Day", ""))))         // sort by day number (not string sort)
        {
            // invoke RunAll on each implementation
            cls?.GetMethod("RunAll")?.Invoke(Activator.CreateInstance(cls), [(day != null || sampleOnly), (day == null || !sampleOnly)]);
        }
        if (day == null)
        {
            Console.WriteLine($"Total time: {sw2.ElapsedMilliseconds}ms");
        }
    }

    private static void ScaffoldClass(int year, int day)
    {
        var path = $"{year}/Day{day}.cs";
        if (File.Exists(path))
        {
            Console.WriteLine($"Unexpected file found at {path}, check class definition!");
            return;
        }

        // create empty {year} and {year}/input paths as needed
        Directory.CreateDirectory($"{year}/inputs");

        var template = File.ReadAllText("DayTemplate.cs");
        template = template.Replace("namespace AOC;", $"namespace AOC.AOC{year};")
            .Replace("DayTemplate", $"Day{day}");
        File.WriteAllText(path, template);

        Console.WriteLine($"Empty implementation written to {path}");
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run [year] [day] [--sampleonly]");
        Console.WriteLine("  year: specific year, or current year if not provided");
        Console.WriteLine("  day:  single day to run (sample and input); if not provided, run all days (input only) for the year and report total time");
        Console.WriteLine("  --sampleonly: if year and day given, run only the sample input");
    }
}
