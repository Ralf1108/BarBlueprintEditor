using BarBlueprintEditor.Shared.Dtos;
using NLua;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LuaToJsonConverter.Converters;

public class LuaToJsonConverter(Dictionary<string, WebUnitDefinition> unitInfos) : IFileConverter
{
    public string GetSearchFilenamePattern()
    {
        return "*.lua";
    }

    public string GetTargetFilenameExtension()
    {
        return ".json";
    }

    public void Convert(string sourceFilePath, string targetFilePath)
    {
        var sourceFilename = Path.GetFileNameWithoutExtension(sourceFilePath);
        if (!unitInfos.ContainsKey(sourceFilename))
            return;

        var content = File.ReadAllText(sourceFilePath);
        var json = ConvertLuaToJson(content);

        // add webInfo
        var jsonRootObject = JsonNode.Parse(json)!.AsObject();

        // check if unit is building
        var jsonObject = jsonRootObject[0] as JsonObject;
        if (jsonObject == null)
            throw new InvalidOperationException("Expected JSON root to be an object with a single property (unit name)");

        if (!jsonObject.TryGetPropertyValue("yardmap", out var yardmap) || yardmap.ToString() == "f")
            return; // skip non-buildings

        var unitName = jsonObject.GetPropertyName(); // first object is name of unit
        if (!unitInfos.TryGetValue(unitName, out var unitInfo))
        {
            Console.WriteLine($"Warning: Couldn't find unit info for '{unitName}' - skipping...");
            return;
        }

        jsonRootObject[unitName]["unitInfo"] = JsonSerializer.SerializeToNode(unitInfo);
        var updatedJson = jsonRootObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        FileWalker.CreateDirectories(targetFilePath);
        File.WriteAllText(targetFilePath, updatedJson);
    }

    private static string ConvertLuaToJson(string luaCode)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;

        //expected: lua code just creates a table and returns it, no function calls or other logic
        var result = lua.DoString(luaCode);
        if (result[0] is not LuaTable luaTable)
            throw new InvalidOperationException("Lua code did not return a table");

        return LuaTableToJson(luaTable);
    }

    private static string LuaTableToJson(LuaTable table, int depth = 0)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', depth * 2);
        sb.Append("{");

        var first = true;
        var keys = table.Keys;
        foreach (var keyObj in keys)
        {
            if (!first)
                sb.Append(",");
            first = false;

            var key = keyObj?.ToString()?.Replace("\"", "\\\"") ?? "";
            sb.Append($"\n{indent}  \"{key}\": ");

            var value = table[keyObj];
            if (value == null)
                sb.Append("null");
            else if (value is bool b)
                sb.Append(b ? "true" : "false");
            else if (value is string s)
                sb.Append($"\"{s.Replace("\"", "\\\"").Replace("\n", "\\n")}\"");
            else if (value is float f)
                sb.Append(f.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (value is double d)
                sb.Append(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (value is int i)
                sb.Append(i.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (value is long l)
                sb.Append(l.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (value is LuaTable subTable)
                sb.Append(LuaTableToJson(subTable, depth + 1));
            else
                throw new InvalidOperationException($"Unsupported Lua value: {value.GetType().Name}]");
        }

        sb.Append($"\n{indent}}}");
        return sb.ToString();
    }
}