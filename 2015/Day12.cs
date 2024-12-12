using Newtonsoft.Json.Linq;

namespace AOC.AOC2015;

public class Day12 : Day<JToken>
{
    protected override string? SampleRawInput { get => "{\"d\":\"red\",\"e\":[1,2,3,4],\"f\":5}"; }

    protected override Answer Part1()
    {
        return SumNumbers(Input);
    }

    protected override Answer Part2()
    {
        return SumNumbers(Input, true);
    }


    private long SumNumbers(JToken token, bool excludeRed = false)
    {
        long sum = 0;

        if (token is JObject obj)
        {
            if (excludeRed && obj.Properties().Any(p => p.Value.Type == JTokenType.String && (string)p.Value! == "red")) return 0;

            foreach (var property in obj.Properties())
            {
                sum += SumNumbers(property.Value, excludeRed);
            }
        }
        else if (token is JArray array)
        {
            foreach (var item in array)
            {
                sum += SumNumbers(item, excludeRed);
            }
        }
        else if (token is JValue value && value.Type == JTokenType.Integer)
            sum += (long)value;

        return sum;
    }

    protected override JToken Parse(string input)
    {
        return JToken.Parse(input);
    }
}