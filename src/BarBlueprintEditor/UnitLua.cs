using System.Text.RegularExpressions;

namespace BarBlueprintEditor;

class UnitLua
{
    private const string KeyFootprintX = "footprintx";
    private const string KeyFootprintY = "footprinty";

    private static readonly Dictionary<string, Regex> Map = new();

    public int FootPrintX { get; init; }
    public int FootPrintY { get; init; }

    static UnitLua()
    {
        AddRegexInt(KeyFootprintX);
        AddRegexInt(KeyFootprintY);
    }

    private static void AddRegexInt(string key)
    {
        Map.Add(key, new Regex($@"{key}\s*=\s*(?<number>\d+)", RegexOptions.IgnoreCase));
    }

    private UnitLua()
    {
    }

    public static UnitLua Parse(string luaText)
    {
        var unit = new UnitLua
        {
            FootPrintX = ReadIntValue(KeyFootprintX, luaText),
            FootPrintY = ReadIntValue(KeyFootprintY, luaText),
        };
        return unit;
    }

    private static int ReadIntValue(string key, string text)
    {
        var match = Map[key].Match(text);
        if (!match.Success)
            throw new InvalidOperationException("Can't find value for key: " + key);

        var value = match.Groups["number"].Value;
        return int.Parse(value);
    }
}