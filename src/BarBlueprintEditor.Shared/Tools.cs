using BarBlueprintEditor.Shared.Dtos;
using BarBlueprintEditor.Shared.Models;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using BarBlueprintEditor.Shared.Extensions;

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

    public record UnitStreamEntry(string Filepath, long Length, Stream Stream);

    public record UnitEntry(UnitDefinition Info, byte[]? Image);

    public static async IAsyncEnumerable<UnitStreamEntry> ExtractZipAsync(Stream stream)
    {
        await using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue; // Skip directories

            await using var entryStream = await entry.OpenAsync();
            yield return new UnitStreamEntry(entry.FullName, entry.Length, entryStream);
        }
    }

    public static async IAsyncEnumerable<UnitEntry> ReadAllZipUnitEntries(Stream zipStream)
    {
        var unitInfos = new Dictionary<string, UnitDefinition>();
        var unitImages = new Dictionary<string, byte[]>();

        await foreach (var entry in ExtractZipAsync(zipStream))
        {
            Debug.WriteLine(entry.Filepath);
            try
            {
                var name = Path.GetFileNameWithoutExtension(entry.Filepath);
                var extension = Path.GetExtension(entry.Filepath);
                switch (extension)
                {
                    case ".json":
                        var unitDefinition = ParseUnitDefinitionFromJson(entry.Stream);
                        unitDefinition.Folder = entry.Filepath;
                        unitInfos[name] = unitDefinition;
                        break;

                    case ".png":
                        var imageBytes = await entry.Stream.ToByteArrayAsync();
                        unitImages[name] = imageBytes;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected file type {extension} in zip entry {entry.Filepath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Can't handle zip entry '{entry.Filepath}' because of error: " + ex.Message);
            }
        }

        foreach (var unitDefinition in unitInfos)
        {
            var image = unitImages.GetValueOrDefault(unitDefinition.Key);
            yield return new UnitEntry(unitDefinition.Value, image);
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
