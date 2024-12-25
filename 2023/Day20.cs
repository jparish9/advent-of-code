using System.Diagnostics.CodeAnalysis;
using AOC.Utils;

namespace AOC.AOC2023;

public class Day20 : Day<Day20.Machine>
{
    //protected override string? SampleRawInput { get => "broadcaster -> a, b, c\n%a -> b\n%b -> c\n%c -> inv\n&inv -> a"; }
    protected override string? SampleRawInput { get => "broadcaster -> a\n%a -> inv, con\n&inv -> b\n%b -> con\n&con -> output"; }

    public enum Pulse { Low, High };

    public class Machine
    {
        public static int TotalRuns { get; set; }
        public static bool IsPart2 { get; set; }
        public static Dictionary<string, long> Part2 { get; } = new Dictionary<string, long>();

        [SetsRequiredMembers]
        public Machine(List<Module> modules)
        {
            Modules = modules.ToDictionary(p => p.Name, p => p);
            Button = (Button)Modules["button"] ?? throw new Exception("No button defined");
        }

        public required Dictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
        private Button Button { get; }
        public int Cycle { get; set; }

        public void Run()
        {
            TotalRuns++;
            Modules.Values.ToList().ForEach(p => p.ClearReceivedPulses());

            Cycle = 0;
            Button.Press();

            while (NextCycle()) {}
        }

        public void ClearAllState()
        {
            Modules.Values.ToList().ForEach(p => p.ClearReceivedPulses());
            Modules.Values.ToList().ForEach(p => p.Reset());
            Part2.Clear();
            TotalRuns = 0;
        }

        public void Print()
        {
            System.Console.WriteLine("Machine definition:");
            foreach (var module in Modules.Values)
            {
                if (module is FlipFlop) System.Console.Write("%");
                else if (module is Conjunction) System.Console.Write("&");
                System.Console.Write($"{module.Name} -> ");
                foreach (var output in module.Outputs)
                {
                    System.Console.Write($" {output.Name}");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        private bool NextCycle()
        {
            Cycle++;

            var todo = Modules.Values.Where(p => p.ReceivedPulses.ContainsKey(Cycle)).ToList();
            foreach (var module in todo)
            {
                module.ProcessReceivedPulses(Cycle);
            }

            return todo.Any();
        }
    }

    public abstract class Module
    {
        public required string Name { get; set; }
        public Dictionary<int, List<Pulse>> ReceivedPulses { get; set; } = new Dictionary<int, List<Pulse>>();          // a module can receive more than one pulse per cycle
        public Dictionary<int, List<Module>> ReceivedPulsesFrom { get; set; } = new Dictionary<int, List<Module>>();    // List<Module> indexes match List<Pulse> above
        public List<Module> Outputs { get; set; } = new List<Module>();

        // counting for part 1
        public int HighPulsesSent = 0;
        public int LowPulsesSent = 0;

        public void SendPulse(int cycle, Pulse pulse, Module destination)
        {
            // check if &xl/&ln/&xp/&gp are sending high and save the first run number that they do (part 2)
            if (Machine.IsPart2 && pulse == Pulse.High && new []{"xl","ln","xp","gp"}.Contains(Name) && !Machine.Part2.ContainsKey(Name))
            {
                Machine.Part2.Add(Name, Machine.TotalRuns);
            }

            if (pulse == Pulse.High) HighPulsesSent++;
            else LowPulsesSent++;

            destination.ReceivePulse(cycle, pulse, this);
        }

        private void ReceivePulse(int cycle, Pulse pulse, Module sender)
        {
            if (!ReceivedPulses.ContainsKey(cycle))
            {
                ReceivedPulses.Add(cycle, new List<Pulse>());
                ReceivedPulsesFrom.Add(cycle, new List<Module>());
            }
            ReceivedPulses[cycle].Add(pulse);
            ReceivedPulsesFrom[cycle].Add(sender);
        }

        public void ClearReceivedPulses()
        {
            ReceivedPulses.Clear();
            ReceivedPulsesFrom.Clear();
        }

        public abstract void Reset();

        public abstract void ProcessReceivedPulses(int cycle);
    }

    public class Broadcaster : Module
    {
        [SetsRequiredMembers]
        public Broadcaster() { Name = "broadcaster"; }

        public override void Reset() {}

        public override void ProcessReceivedPulses(int cycle)
        {
            if (!ReceivedPulses.ContainsKey(cycle)) return;

            // broadcasters send the same pulse they received to all outputs
            ReceivedPulses[cycle].ForEach(p => Outputs.ForEach(q => SendPulse(cycle+1, p, q)));
        }
    }

    public class Conjunction : Module
    {
        public Dictionary<Module, Pulse> InputMemory { get; set; } = new Dictionary<Module, Pulse>();

        public override void Reset()
        {
            foreach (var kv in InputMemory) InputMemory[kv.Key] = Pulse.Low;         // reset all memory to low
        }

        public override void ProcessReceivedPulses(int cycle)
        {
            if (!ReceivedPulses.ContainsKey(cycle)) return;

            // first update our memory for ALL pulses received this cycle
            for (var i=0; i<ReceivedPulses[cycle].Count; i++)
            {
                var pulse = ReceivedPulses[cycle][i];
                var sender = ReceivedPulsesFrom[cycle][i];

                if (!InputMemory.ContainsKey(sender))
                {
                    throw new Exception($"Conjunction {Name} received a pulse from {sender.Name} but has no memory for it");
                }

                InputMemory[sender] = pulse;
            }
            var allHigh = InputMemory.All(p => p.Value == Pulse.High);

            // now process all pulses received this cycle
            // conjunctions send a low pulse if their memory for all input connections is high.
            // note that conjunctions don't care what type of pulse they received.
            ReceivedPulses[cycle].ForEach(p => Outputs.ForEach(q => SendPulse(cycle+1, allHigh ? Pulse.Low : Pulse.High, q)));
        }
    }

    public class FlipFlop : Module
    {
        public enum State { On, Off };
        public State CurrentState { get; set; } = State.Off;

        public override void Reset()
        {
            CurrentState = State.Off;
        }

        public override void ProcessReceivedPulses(int cycle)
        {
            if (!ReceivedPulses.ContainsKey(cycle)) return;

            foreach (var pulse in ReceivedPulses[cycle])
            {
                if (pulse == Pulse.High) continue;        // do nothing

                // for a low pulse, flip our state and send the appropriate pulse to all outputs
                // On -> Off and send low, Off -> On and send high
                CurrentState = CurrentState == State.On ? State.Off : State.On;
                Outputs.ForEach(p => SendPulse(cycle+1, CurrentState == State.On ? Pulse.High : Pulse.Low, p));
            }
        }
    }

    public class Button : Module
    {
        [SetsRequiredMembers]
        public Button() { Name = "button"; }

        public override void Reset() {}

        public void Press()
        {
            if (Outputs.Count != 1 || Outputs[0] is not Broadcaster)
            {
                throw new Exception("Button should only have one connection, and it must be to the broadcaster");
            }
            SendPulse(1, Pulse.Low, Outputs[0]);
        }

        public override void ProcessReceivedPulses(int cycle)
        {
            throw new Exception("Button should never receive a pulse!");
        }
    }

    // an output module can have any name, not just "output" as in the sample!
    public class Output : Module
    {
        public override void Reset() {}

        public override void ProcessReceivedPulses(int cycle) {}
    }

    protected override Answer Part1()
    {
        Input.ClearAllState();
        for (var i=0; i<1000; i++)
        {
            Input.Run();
        }

        return Input.Modules.Sum(p => p.Value.HighPulsesSent) * Input.Modules.Sum(p => p.Value.LowPulsesSent);
    }

    protected override Answer Part2()
    {
        if (!Input.Modules.ContainsKey("rx")) return -1;            // sample input may not have rx output, can't run part 2 in that case

        // after some testing, brute force was not going to work.
        // I looked at the inputs to rx (there is only one, &df).
        // df is a conjunction of 4 inputs: &xl, &ln, &xp, &gp, all also conjunctions.
        // the end condition is a low pulse to rx (sent by df), which is only sent when all 4 of its inputs are high.
        // after some more testing, each of these 4 inputs are high with their own independent (and relatively prime) cycle lengths, and that cycle starts from 0 (i.e. there is no "warmup" period before the Machine enters a loop).
        // so we need to find the first cycle where all 4 inputs are high, and then the cycle length (when df gets a low pulse) is the lcm of the 4 cycle lengths.
        // push buttons until all four inputs have output high once, save off those run numbers, then lcm them.
        // I also ran into an off-by-2000 bug (lol) where I had the lcm part right, actually impelemented the chinese remainder theorem (which gave the right answer if there actually WAS a warmup period of 2000 cycles),
        // but finally realized that I hadn't reset the machine between parts!  the 2000 was from running part 1 (x1000) twice.

        Input.ClearAllState();
        Machine.IsPart2 = true;
        while (Machine.Part2.Count < 4)
        {
            Input.Run();
        }

        return Machine.Part2.Values.Aggregate(Maths.LeastCommonMultiple);
    }

    protected override Machine Parse(RawInput input)
    {
        Broadcaster? broadcaster = null;

        var allModules = new Dictionary<string, Module>();
        var destinations = new Dictionary<Module, List<string>>();          // for wiring up once we have all modules defined

        foreach (var line in input.Lines())
        {
            var parts = line.Split(" -> ");
            var source = parts[0];
            var destination = parts[1];

            Module? sourceModule = null;
            if (source.StartsWith("&"))
            {
                sourceModule = new Conjunction() { Name = source[1..] };
            }
            else if (source.StartsWith("%"))
            {
                sourceModule = new FlipFlop() { Name = source[1..] };
            }
            else if (source.StartsWith("broadcaster"))
            {
                if (broadcaster != null) { throw new Exception("Only one broadcaster allowed"); }
                broadcaster = new Broadcaster();
                sourceModule = broadcaster;
            }
            else
            {
                throw new Exception($"Unknown module type {source}");
            }

            allModules.Add(sourceModule!.Name, sourceModule!);
            destinations.Add(sourceModule!, destination.Split(", ").ToList());
        }

        if (broadcaster == null)
        {
            throw new Exception("No broadcaster defined");
        }

        // wire up connections
        foreach (var kvp in destinations)
        {
            var module = kvp.Key;

            foreach (var destinationName in kvp.Value)
            {
                if (!allModules.ContainsKey(destinationName))
                {
                    // otherwise-undefined output module; create it and add it to allModules
                    var output = new Output() { Name = destinationName };
                    module.Outputs.Add(output);
                    allModules.Add(output.Name, output);
                }
                else
                {
                    var destination = allModules[destinationName];
                    module.Outputs.Add(destination);
                }
            }
        }

        // wire up conjunction inputs
        foreach (var conjunction in allModules.Where(p => p.Value is Conjunction).Select(p => p.Value).Cast<Conjunction>())
        {
            allModules.Where(p => p.Value.Outputs.Contains(conjunction)).ToList().ForEach(p => conjunction.InputMemory.Add(p.Value, Pulse.Low));
        }

        // wire up a button to the broadcaster
        var button = new Button();
        button.Outputs.Add(broadcaster);
        allModules.Add(button.Name, button);

        return new Machine(allModules.Values.ToList());
    }
}

/*      chinese remainder theorem example
        var test = new List<long>() { 49, 53, 55, 62 };
        var nums = new List<long>() { 5, 3, 2, 7};
        var lcm = nums.Aggregate(LeastCommonMultiple);

        var a = test.Select((p, i) => test[i] % nums[i]).ToList();
        var z = nums.Select(p => lcm / p).ToList();
        var y = z.Select((p, i) => MultiplicativeInverse(p, nums[i])).ToList();
        var w = y.Select((p, i) => p * z[i] % lcm).ToList();

        System.Console.WriteLine("M = " + string.Join(" ", a));
        System.Console.WriteLine("Z = " + string.Join(" ", z));
        System.Console.WriteLine("Y = " + string.Join(" ", y));
        System.Console.WriteLine("W = " + string.Join(" ", w));

        var x = w.Select((p, i) => p * a[i]).Sum() % lcm;
        System.Console.WriteLine(x);

        while (x < test.Max()) x += lcm;

        System.Console.WriteLine(x);


    private int MultiplicativeInverse(long val, long modulus)
    {
        for (var i=1; i<modulus; i++)
        {
            if ((val * i) % modulus == 1) return i;
        }

        throw new Exception("No multiplicative inverse found");
    }*/