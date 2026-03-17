using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarFileConverter;

/// <summary>
/// Full custom recursive JSON Schema inferencer for multiple JSON documents.
/// Behaves very similarly to Python's "genson" library:
///   - Properties are required ONLY if they appear in EVERY document
///   - Conflicting types → anyOf
///   - Arrays with different item types → items anyOf
///   - Nulls are handled correctly
///   - No external dependencies (pure System.Text.Json)
/// Works on .NET 8 / 9 / 10 (and higher).
/// </summary>
public static class JsonSchemaMerger
{
    /// <summary>
    /// generate schema from all *.json files in a folder.
    /// </summary>
    public static string GenerateFromFolder(
        string folderPath,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        string schemaTitle = "CommonSchema")
    {
        var elements = Directory.GetFiles(folderPath, "*.json", searchOption)
            .Select(x =>
            {
                var content = File.ReadAllText(x);
                return JsonDocument.Parse(content).RootElement;
            })
            .ToList();

        var schema = InferSchema(elements);
        schema["title"] = schemaTitle;
        schema["$schema"] = "https://json-schema.org/draft/2020-12/schema";

        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(schema, options);
    }

    private static JsonObject InferSchema(IEnumerable<JsonElement> values)
    {
        if (!values.Any())
            return new JsonObject { ["type"] = "object" };

        // Separate nulls
        var nonNullValues = values.Where(v => v.ValueKind != JsonValueKind.Null).ToList();
        var hasNull = values.Count() != nonNullValues.Count;
        if (nonNullValues.Count == 0)
            return new JsonObject { ["type"] = "null" };

        // Group by ValueKind
        var groups = nonNullValues
            .GroupBy(v => v.ValueKind)
            .ToDictionary(g => g.Key, g => g.ToList());
        var typeSchemas = new List<JsonObject>();
        foreach (var (kind, groupValues) in groups)
        {
            typeSchemas.Add(BuildSchemaForKind(kind, groupValues));
        }

        JsonObject rootSchema;
        if (typeSchemas.Count == 1 || typeSchemas.Select(x => x.ToJsonString()).Distinct().Count() == 1)
        {
            rootSchema = typeSchemas[0];
        }
        else
        {
            rootSchema = new JsonObject
            {
                ["anyOf"] = new JsonArray(typeSchemas.Select(s => s).ToArray())
            };
        }

        // Handle nullability
        if (hasNull)
        {
            if (typeSchemas.Count == 1 && !rootSchema.ContainsKey("anyOf"))
            {
                // Prefer compact "type": ["string", "null"] when possible
                var currentTypeNode = rootSchema["type"];
                if (currentTypeNode is JsonValue currentTypeValue)
                {
                    rootSchema["type"] = new JsonArray { currentTypeValue, "null" };
                }
                else if (currentTypeNode is JsonArray currentTypeArray)
                {
                    var newArray = new JsonArray(currentTypeArray.Select(n => n).ToArray());
                    newArray.Add("null");
                    rootSchema["type"] = newArray;
                }
            }
            else
            {
                // Add to anyOf
                var anyOfArray = rootSchema["anyOf"] as JsonArray ?? new JsonArray(rootSchema);
                anyOfArray.Add(new JsonObject { ["type"] = "null" });
                rootSchema["anyOf"] = anyOfArray;
            }
        }

        return rootSchema;
    }

    private static JsonObject BuildSchemaForKind(JsonValueKind kind, List<JsonElement> values)
    {
        switch (kind)
        {
            case JsonValueKind.String:
                return new JsonObject { ["type"] = "string" };

            case JsonValueKind.Number:
                var allIntegers = values.All(IsInteger);
                return new JsonObject { ["type"] = allIntegers ? "integer" : "number" };

            case JsonValueKind.True:
            case JsonValueKind.False:
                return new JsonObject { ["type"] = "boolean" };

            case JsonValueKind.Object:
                return BuildObjectSchema(values);

            case JsonValueKind.Array:
                return BuildArraySchema(values);

            default:
                return new JsonObject { ["type"] = "null" };
        }
    }

    private static JsonObject BuildObjectSchema(List<JsonElement> objectValues)
    {
        if (!objectValues.Any())
            return new JsonObject { ["type"] = "object" };

        var propValues = new Dictionary<string, List<JsonElement>>();
        HashSet<string> required = null!;
        var first = true;
        foreach (var obj in objectValues)
        {
            var currentProps = new HashSet<string>();

            foreach (var prop in obj.EnumerateObject())
            {
                currentProps.Add(prop.Name);

                if (!propValues.TryGetValue(prop.Name, out var list))
                {
                    list = new List<JsonElement>();
                    propValues[prop.Name] = list;
                }
                list.Add(prop.Value);
            }

            if (first)
            {
                required = currentProps;
                first = false;
            }
            else
            {
                required.IntersectWith(currentProps);
            }
        }

        var properties = new JsonObject();
        foreach (var (name, vals) in propValues)
        {
            properties[name] = InferSchema(vals); // recursive call
        }

        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["additionalProperties"] = false
        };

        if (required.Count > 0)
        {
            var reqArray = new JsonArray(required.OrderBy(x => x).Select(r => (JsonNode)r).ToArray());
            schema["required"] = reqArray;
        }

        return schema;
    }

    private static JsonObject BuildArraySchema(List<JsonElement> arrayValues)
    {
        var allItems = new List<JsonElement>();
        foreach (var arr in arrayValues)
        {
            foreach (var item in arr.EnumerateArray())
            {
                allItems.Add(item);
            }
        }

        var itemsSchema = allItems.Any()
            ? InferSchema(allItems)           // recursive
            : new JsonObject();               // any value allowed
        return new JsonObject
        {
            ["type"] = "array",
            ["items"] = itemsSchema
        };
    }

    private static bool IsInteger(JsonElement e)
    {
        if (e.TryGetInt64(out _))
            return true;
        if (e.TryGetDouble(out var d))
            return Math.Abs(d - Math.Floor(d)) < double.Epsilon; // safe integer check
        return false;
    }
}