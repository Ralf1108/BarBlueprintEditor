namespace BarBlueprintEditor.Shared.Models;

public class UnitDefinition
{
    public string Name { get; init; }
    public string Folder { get; set; }

    public float FootprintX { get; init; }
    public float FootprintZ { get; init; }
}