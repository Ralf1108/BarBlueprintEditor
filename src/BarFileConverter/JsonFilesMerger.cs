using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarFileConverter;

public class JsonFilesMerger
{
    public static string Merge(string folderPath)
    {
        JsonNode? mergedNode = null;
        foreach (var file in Directory.GetFiles(folderPath, "*.json"))
        {
            var json = File.ReadAllText(file);
            var node = JsonNode.Parse(json);

            if (mergedNode == null)
            {
                mergedNode = node;
            }
            else
            {
                MergeNodes(mergedNode, node);
            }
        }

        if (mergedNode == null)
            throw new InvalidOperationException();

        return mergedNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

    // Naive recursive merge helper
    private static void MergeNodes(JsonNode target, JsonNode source)
    {
        if (source is JsonObject srcObj && target is JsonObject tgtObj)
        {
            foreach (var kv in srcObj)
            {
                if (!tgtObj.ContainsKey(kv.Key))
                {
                    tgtObj[kv.Key] = kv.Value?.DeepClone();
                }
                else if (kv.Value is JsonObject || kv.Value is JsonArray)
                {
                    MergeNodes(tgtObj[kv.Key]!, kv.Value!);
                }
                // else value type → keep first occurrence or implement your policy
            }
        }
        else if (source is JsonArray srcArr && target is JsonArray tgtArr)
        {
            // Union-like → simplistic append (improve as needed)
            foreach (var item in srcArr)
                tgtArr.Add(item?.DeepClone());
        }
    }
}