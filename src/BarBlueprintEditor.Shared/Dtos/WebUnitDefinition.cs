namespace BarBlueprintEditor.Shared.Dtos;

public class WebUnitDefinition(
    string Name,
    string ImageUrl,
    string Title,
    string Description,
    string TechLevel)
{
    public string Name { get; init; } = Name;
    public string ImageUrl { get; init; } = ImageUrl;
    public string Title { get; init; } = Title;
    public string Description { get; init; } = Description;
    public string TechLevel { get; init; } = TechLevel;
}