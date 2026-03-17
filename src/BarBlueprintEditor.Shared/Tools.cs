using System.IO.Compression;
using System.Text;
using System.Text.Json;
using BarBlueprintEditor.Shared.Dtos;
using BarBlueprintEditor.Shared.Models;

namespace BarBlueprintEditor.Shared;

public static class Tools
{
    public static List<Blueprint> ParseFromBlueprints(string blueprintText)
    {
        var root = JsonSerializer.Deserialize<BlueprintRoot>(blueprintText);
        return root!.Blueprints;
    }

    public static UnitDefinition ParseUnitDefinitionFromJson(Stream stream)
    {
        var entry = JsonSerializer.Deserialize<BarUnit>(stream);
        var unitDefinition = entry.ExtraUnitInfo;
        return new UnitDefinition
        {
            Name = unitDefinition.Name,
            Title = unitDefinition.Title,
            Description = unitDefinition.Description,
            TechLevel = unitDefinition.TechLevel,
            ImageUrl = unitDefinition.ImageUrl,
            
            FootprintX = entry.Footprintx,
            FootprintZ = entry.Footprintz
        };
    }

    public record UnitStreamEntry(string Filepath, Stream Stream);

    public static async IAsyncEnumerable<UnitStreamEntry> ExtractZipAsync(Stream stream)
    {
        await using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue; // Skip directories
            
            await using var entryStream = await entry.OpenAsync();
            yield return new UnitStreamEntry(entry.FullName, entryStream);
        }
    }

    public static string ReadAllTextUtf8(Stream stream)
    {
        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 16384,
            leaveOpen: true);
        return reader.ReadToEnd();
    }
}
