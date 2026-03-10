using System.Text.Json.Serialization;

namespace BarBlueprintEditor;

public class BlueprintRoot
{
    [JsonPropertyName("savedBlueprints")]
    public List<Blueprint> Blueprints { get; set; }
}

public class Blueprint
{
    [JsonPropertyName("ordered")]
    public bool Ordered { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("spacing")]
    public int Spacing { get; set; }

    [JsonPropertyName("facing")]
    public int Facing { get; set; }

    [JsonPropertyName("units")]
    public List<Unit> Units { get; set; }
}

public class Unit
{
    [JsonPropertyName("unitName")]
    public string UnitName { get; set; }

    [JsonPropertyName("facing")]
    public int Facing { get; set; }

    [JsonPropertyName("position")]
    public List<double> Position { get; set; }
}