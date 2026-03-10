using System.Text.Json;
using System.Text.RegularExpressions;

namespace BarBlueprintEditor;

public class Tools
{
    public static List<Blueprint> ParseFromBlueprints(string blueprintText)
    {
        var root = JsonSerializer.Deserialize<BlueprintRoot>(blueprintText);
        return root!.Blueprints;
    }

    public static UnitDefinition ParseFromLuaDefinition(string luaText)
    {
        var json = ConvertLuaDefinitionToJson(luaText);
        //var root = JsonSerializer.Deserialize<LuaRoot>(json, new JsonSerializerOptions{AllowTrailingCommas = true});
        //return root!.Units.Single().Value;
        var root = JsonSerializer.Deserialize<Dictionary<string, LuaUnit>>(json, new JsonSerializerOptions { AllowTrailingCommas = true });
        var entry = root!.Single();
        return new UnitDefinition(entry.Key, entry.Value);
    }

    private static string ConvertLuaDefinitionToJson(string luaText)
    {
        var rep = Regex.Replace(luaText, @"return {", @"{");
        rep = Regex.Replace(rep, "\t*(.+) =", "\"$1\" :");
        return rep;
    }

    /// <summary>
    /// better use git clone, zip files and upload them to cdn or compile it into web app
    /// </summary>
    /// <returns></returns>
    private static async Task DownloadAllBarUnitsLua()
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("MyLuaDownloader/1.0"); // GitHub requires User-Agent

        // Optional: authenticated (60 → 5000 req/h)
        // http.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", "ghp_YourPersonalAccessTokenHere");

        var url = "https://api.github.com/repos/beyond-all-reason/Beyond-All-Reason/git/trees/master?recursive=1";

        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var luaFiles = new List<(string Path, string Sha, string Url)>();
        if (root.TryGetProperty("tree", out var tree))
        {
            foreach (var item in tree.EnumerateArray())
            {
                var path = item.GetProperty("path").GetString()!;
                var type = item.GetProperty("type").GetString()!;

                if (type != "blob" || !path.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
                    continue;

                var sha = item.GetProperty("sha").GetString()!;
                var downloadUrl =
                    $"https://raw.githubusercontent.com/beyond-all-reason/Beyond-All-Reason/master/{path}";

                luaFiles.Add((path, sha, downloadUrl));
                //Console.WriteLine($"Found: {path}");
            }
        }

        //Console.WriteLine($"\nTotal .lua files found: {luaFiles.Count}");

        // Example: download one of them
        if (luaFiles.Count > 0)
        {
            var first = luaFiles[0];
            //Console.WriteLine($"\nDownloading first file: {first.Path}");

            var content = await http.GetStringAsync(first.Url);
            // or await http.GetByteArrayAsync(...) for binary safety

            await System.IO.File.WriteAllTextAsync($"units_{first.Path.Replace("/", "_")}", content);
            //Console.WriteLine("Downloaded.");
        }

    }
}
