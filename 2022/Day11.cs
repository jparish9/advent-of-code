namespace AOC.AOC2022;

public class Day11 : Day<Day11.MonkeyGroup>
{
    protected override string? SampleRawInput { get => "Monkey 0:\n  Starting items: 79, 98\n  Operation: new = old * 19\n  Test: divisible by 23\n    If true: throw to monkey 2\n    If false: throw to monkey 3\n\nMonkey 1:\n  Starting items: 54, 65, 75, 74\n  Operation: new = old + 6\n  Test: divisible by 19\n    If true: throw to monkey 2\n    If false: throw to monkey 0\n\nMonkey 2:\n  Starting items: 79, 60, 97\n  Operation: new = old * old\n  Test: divisible by 13\n    If true: throw to monkey 1\n    If false: throw to monkey 3\n\nMonkey 3:\n  Starting items: 74\n  Operation: new = old + 3\n  Test: divisible by 17\n    If true: throw to monkey 0\n    If false: throw to monkey 1"; }

    public class MonkeyGroup
    {
        public MonkeyGroup Copy()
        {
            return new MonkeyGroup()
            {
                Monkeys = Monkeys.Select(p => new Monkey()
                {
                    Items = new Queue<long>(p.Items),
                    Operation = p.Operation,
                    Operand = p.Operand,
                    DivisibilityTest = p.DivisibilityTest,
                    IfTrueMonkeyId = p.IfTrueMonkeyId,
                    IfFalseMonkeyId = p.IfFalseMonkeyId,
                    Inspected = 0L
                }).ToList()
            };
        }

        public required List<Monkey> Monkeys { get; set; }

        public long Run(int rounds, int worryReductionFactor)
        {
            long numberSpace = Monkeys.Select(p => (long)p.DivisibilityTest).Distinct().Aggregate((p, q) => p * q);

            for (var i=0; i<rounds; i++)
            {
                foreach (var monkey in Monkeys)
                {
                    while (monkey.Items.Any())
                    {
                        monkey.Inspected++;
                        var item = monkey.Items.Dequeue();
                        var result = monkey.Operation switch
                        {
                            '+' => item + monkey.Operand,
                            '*' => item * monkey.Operand,
                            '^' => (long)Math.Pow(item, monkey.Operand),
                            _ => throw new Exception("Unknown operation")
                        };

                        if (worryReductionFactor != 1) result /= worryReductionFactor;            // integer division

                        var to = result % monkey.DivisibilityTest == 0 ? monkey.IfTrueMonkeyId : monkey.IfFalseMonkeyId;

                        result %= numberSpace;

                        Monkeys[to].Items.Enqueue(result);
                    }
                }
            }

            return Monkeys.Select(p => p.Inspected).OrderByDescending(p => p).Take(2).Aggregate((p, q) => p * q);
        }
    }

    public class Monkey
    {
        public required Queue<long> Items { get; set; }
        public char Operation { get; set; }
        public int Operand { get; set; }
        public int DivisibilityTest { get; set; }
        public int IfTrueMonkeyId { get; set; }
        public int IfFalseMonkeyId { get; set; }
        public long Inspected { get; set;}
    }

    protected override long Part1()
    {
        return Input.Copy().Run(20, 3);
    }

    protected override long Part2()
    {
        return Input.Copy().Run(10000, 1);
    }

    protected override MonkeyGroup Parse(string input)
    {
        var monkeys = input.Split("\n\n").Select(p => p.Trim()).ToList();

        var monkeyList = new List<Monkey>();

        foreach (var monkey in monkeys)
        {
            var lines = monkey.Split("\n");
            var startingItems = lines[1].Split(":")[1].Trim().Split(",").Select(p => long.Parse(p.Trim())).ToList();
            var opLine = lines[2].Split("=")[1].Trim().Split(" ");
            var operation = opLine[1];
            var operand = opLine[2];
            if (operation == "*" && operand == "old") { operation = "^"; operand = "2"; };
            var divisible = lines[3].Split(" ")[^1];
            var ifTrue = lines[4].Split(" ")[^1];
            var ifFalse = lines[5].Split(" ")[^1];

            monkeyList.Add(new Monkey()
            {
                Items = new Queue<long>(startingItems),
                Operation = operation[0],
                Operand = int.Parse(operand),
                DivisibilityTest = int.Parse(divisible),
                IfTrueMonkeyId = int.Parse(ifTrue),
                IfFalseMonkeyId = int.Parse(ifFalse)
            });
        }

        return new MonkeyGroup() { Monkeys = monkeyList };
    }
}