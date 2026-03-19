namespace BarBlueprintEditor.Shared.Models;

public class UnitDefinition
{
    public string Name { get; init; }
    public string Folder { get; set; }

    public string Title { get; init; }
    public string Description { get; init; }
    public string TechLevel { get; init; }
    public string ImageUrl { get; init; }

    public int FootprintX { get; init; }
    public int FootprintZ { get; init; }
}