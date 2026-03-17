using System.Text.Json.Serialization;

namespace BarBlueprintEditor.Shared.Dtos;

public class WebUnitDefinition()
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("techLevel")]
    public string TechLevel { get; set; } = string.Empty;
}