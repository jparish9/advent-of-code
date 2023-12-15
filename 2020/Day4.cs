namespace AOC.AOC2020;

public class Day4 : Day<List<Day4.Passport>>
{
    protected override string? SampleRawInput { get => "ecl:gry pid:860033327 eyr:2020 hcl:#fffffd\nbyr:1937 iyr:2017 cid:147 hgt:183cm\n\niyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884\nhcl:#cfa07d byr:1929\n\nhcl:#ae17e1 iyr:2013\neyr:2024\necl:brn pid:760753108 byr:1931\nhgt:179cm\n\nhcl:#cfa07d eyr:2025 pid:166559648\niyr:2011 ecl:brn hgt:59in"; }

    public class Passport
    {
        public required Dictionary<string, string> Fields { get; set; }
    }

    protected override long Part1()
    {
        // passport is valid if it contains all fields except cid
        return Input.Count(p => p.Fields.Count == 8 || (p.Fields.Count == 7 && !p.Fields.ContainsKey("cid")));
    }

    protected override long Part2()
    {
        // part 1, plus additional validation rules
        var ct = 0;
        foreach (var p in Input)
        {
            if (p.Fields.Count != 8 && (p.Fields.Count != 7 || p.Fields.ContainsKey("cid"))) continue;

            var byr = int.Parse(p.Fields["byr"]);
            if (byr < 1920 || byr > 2002) continue;

            var iyr = int.Parse(p.Fields["iyr"]);
            if (iyr < 2010 || iyr > 2020) continue;

            var eyr = int.Parse(p.Fields["eyr"]);
            if (eyr < 2020 || eyr > 2030) continue;

            var hgt = p.Fields["hgt"];
            if (hgt.EndsWith("cm"))
            {
                var cm = int.Parse(hgt[..^2]);
                if (cm < 150 || cm > 193) continue;
            }
            else if (hgt.EndsWith("in"))
            {
                var inch = int.Parse(hgt[..^2]);
                if (inch < 59 || inch > 76) continue;
            }
            else continue;

            var hcl = p.Fields["hcl"];
            if (hcl.Length != 7 || hcl[0] != '#') continue;
            if (!int.TryParse(hcl.AsSpan(1), System.Globalization.NumberStyles.HexNumber, null, out _)) continue;

            var ecl = p.Fields["ecl"];
            if (!new string[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(ecl)) continue;

            var pid = p.Fields["pid"];
            if (pid.Length != 9 || !int.TryParse(pid, out _)) continue;

            ct++;
        }

        return ct;
    }

    protected override List<Passport> Parse(string input)
    {
        var passports = new List<Passport>();

        var each = input.Split("\n\n");
        foreach (var p in each)
        {
            var fields = new Dictionary<string, string>();
            foreach (var f in p.Split(' ', '\n'))
            {
                var kv = f.Split(':');
                if (kv.Length != 2) continue;
                fields.Add(kv[0], kv[1]);
            }
            passports.Add(new Passport { Fields = fields });
        }

        return passports;
    }
}