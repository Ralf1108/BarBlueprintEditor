using System.Text.Json.Serialization;

namespace BarBlueprintEditor.Shared.Dtos;

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
    public List<BlueprintUnit> Units { get; set; }
}

public class BlueprintUnit
{
    [JsonPropertyName("unitName")]
    public string UnitName { get; set; }

    [JsonPropertyName("facing")]
    public int Facing { get; set; }

    [JsonPropertyName("position")]
    public List<double> Position { get; set; }
}