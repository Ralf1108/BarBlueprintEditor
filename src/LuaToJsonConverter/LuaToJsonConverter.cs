using System.Text;
using NLua;

namespace LuaToJsonConverter;

public static class Converter
{
    public static void Convert(string sourceFolder, string targetFolder)
    {
        var fullSourceFolderPath = Path.GetFullPath(sourceFolder);
        var fullTargetFolderPath = Path.GetFullPath(targetFolder);

        foreach (var filePath in Directory.EnumerateFiles(sourceFolder, "*.lua", SearchOption.AllDirectories))
        {
            var fullFilePath = Path.GetFullPath(filePath);
            var content = File.ReadAllText(fullFilePath);
            string json;
            try
            {
                json = ConvertLuaToJson(content);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting file '{fullFilePath}' to json: " + ex.Message, ex);
            }

            var subPath = Path.GetDirectoryName(fullFilePath)!.Replace(fullSourceFolderPath, "").Trim("\\").ToString();
            var targetFile = Path.ChangeExtension(Path.GetFileName(fullFilePath), ".json");
            var targetFilePath = Path.Combine(fullTargetFolderPath, subPath, targetFile);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath)!);
            File.WriteAllText(targetFilePath, json);
        }
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