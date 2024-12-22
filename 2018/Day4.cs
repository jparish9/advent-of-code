namespace AOC.AOC2018;

public class Day4 : Day<Day4.GuardLog>
{
    protected override string? SampleRawInput { get => "[1518-11-01 00:00] Guard #10 begins shift\n[1518-11-01 00:05] falls asleep\n[1518-11-01 00:25] wakes up\n[1518-11-01 00:30] falls asleep\n[1518-11-01 00:55] wakes up\n[1518-11-01 23:58] Guard #99 begins shift\n[1518-11-02 00:40] falls asleep\n[1518-11-02 00:50] wakes up\n[1518-11-03 00:05] Guard #10 begins shift\n[1518-11-03 00:24] falls asleep\n[1518-11-03 00:29] wakes up\n[1518-11-04 00:02] Guard #99 begins shift\n[1518-11-04 00:36] falls asleep\n[1518-11-04 00:46] wakes up\n[1518-11-05 00:03] Guard #99 begins shift\n[1518-11-05 00:45] falls asleep\n[1518-11-05 00:55] wakes up"; }

    public class GuardLog
    {
        public required List<LogEntry> LogEntries;

        public Dictionary<(int GuardId, int Minute), int> Asleep = [];

        public void Analyze()
        {
            foreach (var entry in LogEntries)
            {
                if (entry.Action == Action.FallAsleep)
                {
                    var wake = LogEntries.First(p => p.Time > entry.Time && p.Action == Action.WakeUp);

                    for (var i=entry.Time.Minute; i<wake.Time.Minute; i++)
                    {
                        if (!Asleep.ContainsKey((entry.GuardId, i))) Asleep.Add((entry.GuardId, i), 0);
                        Asleep[(entry.GuardId, i)]++;
                    }
                }
            }
        }
    }

    public class LogEntry
    {
        public required DateTime Time;
        public int GuardId;
        public required Action Action;
    }
    
    public enum Action
    {
        BeginShift,
        FallAsleep,
        WakeUp
    }

    protected override Answer Part1()
    {
        Input.Analyze();

        // find the guard that has the most minutes asleep
        var guardId = Input.Asleep.GroupBy(p => p.Key.GuardId)
            .Select(p => new { GuardId = p.Key, Minutes = p.Sum(q => q.Value) })
            .Aggregate((a, b) => a.Minutes > b.Minutes ? a : b)
            .GuardId;

        // and what minute that guard was the most asleep
        var minute = Input.Asleep.Where(p => p.Key.GuardId == guardId)
            .Aggregate((a, b) => a.Value > b.Value ? a : b)
            .Key.Minute;

        return guardId * minute;
    }

    protected override Answer Part2()
    {
        // analysis cached from part 1
        var max = Input.Asleep.Aggregate((a, b) => a.Value > b.Value ? a : b);

        return max.Key.GuardId * max.Key.Minute;
    }

    protected override GuardLog Parse(string input)
    {
        var rows = input.Split("\n").Where(p => p != "").Select(p => p.Split("] ")).ToList();

        // sort the entries first
        var tmp = new List<(DateTime Timestamp, string Log)>();
        foreach (var row in rows)
        {
            var dt = DateTime.Parse(row[0][1..]);
            tmp.Add((dt, row[1]));
        }
        tmp.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

        // now we can fill in the guard ids for falls asleep/wakes up
        var logEntries = new List<LogEntry>();
        var currentGuard = -1;
        foreach (var (Timestamp, Log) in tmp)
        {
            Action action;
            if (Log.StartsWith("Guard"))
            {
                currentGuard = int.Parse(Log.Split(" ")[1][1..]);
                action = Action.BeginShift;
            }
            else if (Log == "falls asleep")
            {
                action = Action.FallAsleep;
            }
            else if (Log == "wakes up")
            {
                action = Action.WakeUp;
            }
            else
            {
                throw new Exception("Unknown log entry");
            }

            logEntries.Add(new LogEntry() { Time = Timestamp, GuardId = currentGuard, Action = action });
        }

        return new GuardLog() { LogEntries = logEntries };
    }
}