using System.Data;

namespace AOC.AOC2024;

public class Day5 : Day<Day5.Manual>
{
    protected override string? SampleRawInput { get => "47|53\n97|13\n97|61\n97|47\n75|29\n61|13\n75|53\n29|13\n97|29\n53|29\n61|53\n97|53\n61|29\n47|13\n75|47\n97|75\n47|61\n75|61\n47|29\n75|13\n53|13\n\n75,47,61,53,29\n97,61,53,29,13\n75,29,13\n75,97,47,61,53\n61,13,29\n97,13,75,29,47"; }

    public class Manual
    {
        public required List<Update> Updates;
        public required Rules Rules;
    }

    public class Rules
    {
        public Rules(List<RuleDef> rules)
        {
            RuleDefs = rules;
            FirstMap = rules.GroupBy(p => p.First).ToDictionary(p => p.Key, p => p.ToList());
            SecondMap = rules.GroupBy(p => p.Second).ToDictionary(p => p.Key, p => p.ToList());
        }

        public List<RuleDef> RuleDefs;

        // maps for performance
        private readonly Dictionary<int, List<RuleDef>> FirstMap;
        private readonly Dictionary<int, List<RuleDef>> SecondMap;

        public List<RuleDef> FirstRules(int page)
        {
            return FirstMap.ContainsKey(page) ? FirstMap[page] : new List<RuleDef>();
        }

        public List<RuleDef> SecondRules(int page)
        {
            return SecondMap.ContainsKey(page) ? SecondMap[page] : new List<RuleDef>();
        }
    }

    public class RuleDef
    {
        public int First;
        public int Second;
    }

    public class Update
    {
        public required List<int> Pages;
        public bool IsCorrect;
        public bool Check(Rules rules)
        {
            IsCorrect = true;
            for (var i=0; i<Pages.Count; i++)
            {
                if (rules.FirstRules(Pages[i]).Any(p => Pages.Take(i).Contains(p.Second))
                    || rules.SecondRules(Pages[i]).Any(p => Pages.Skip(i+1).Contains(p.First)))
                    {
                        IsCorrect = false;
                        break;
                    }
            }
            return IsCorrect;
        }

        public int Median() { return Pages[Pages.Count/2]; }
    }

    protected override long Part1()
    {
        var sum = 0;
        foreach (var update in Input.Updates)
        {
            if (update.Check(Input.Rules))
                sum += update.Median();
        }

        return sum;
    }

    protected override long Part2()
    {
        var sum = 0;

        // my first try swapped pages according to violated rules and rechecked until correct.
        // this was effectively a bubblesort, and although fine for this input (~0.5s), there is a better way that uses native sort,
        // and also does not need to keep track of violated rules or recheck at every reordering.
        // the set of rules is effectively a comparator that can be used with native sorting.
        foreach (var update in Input.Updates.Where(p => !p.IsCorrect))
        {
            update.Pages.Sort(new Comparison<int>((a, b) => {
                if (Input.Rules.FirstRules(a).Select(p => p.Second).Contains(b))
                    return -1;
                else return 1;
            }));

            sum += update.Median();
        }

        return sum;
    }

    protected override Manual Parse(string input)
    {
        var parts = input.Split("\n\n");
        return new Manual()
        {
            Rules = new Rules(parts[0].Split("\n").Where(p => p != "").Select(p => p.Split("|").Select(int.Parse).ToList()).Select(p => new RuleDef() { First = p[0], Second = p[1] }).ToList()),
            Updates = parts[1].Split("\n").Where(p => p != "").Select(p => p.Split(",").Select(int.Parse).ToList()).Select(p => new Update() { Pages = p }).ToList()
        };
    }
}