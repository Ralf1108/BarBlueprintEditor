using BarBlueprintEditor.Shared.Dtos;
using BarFileConverter.Converters;
using System.CommandLine;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BarFileConverter;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var sourceFolderOption = new Option<string>("--source", "-s")
        {
            Description = "Specifies the source folder"
        };

        var targetFolderOption = new Option<string>("--target", "-t")
        {
            Description = "Specifies the target folder"
        };

        var targetFilePathOption = new Option<string>("--target", "-t")
        {
            Description = "Specifies the target file path"
        };

        var sourceFilePathOption = new Option<string>("--source", "-s")
        {
            Description = "Specifies the source file path",
            Required = true
        };

        var unitDefinitionsFilePathOption = new Option<string>("--unitDefinitions", "-ud")
        {
            Description = "Specifies the unit definitions filepath",
            Required = true
        };

        if (args.Length == 0)
        {
            args =
            [
                //@"web",
                //targetFilePathOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\webUnitDefinitions.json",

                //@"dds",
                //sourceFolderOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\git\unitpics",
                //targetFolderOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\converted\unitpics",
                //unitDefinitionsFilePathOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\webUnitDefinitions.json",
                
                //@"lua",
                //sourceFolderOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\git\units",
                //targetFolderOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\converted\units",
                //unitDefinitionsFilePathOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\webUnitDefinitions.json",

                //@"schema",
                //sourceFolderOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\converted\units",
                //targetFilePathOption.Name,
                //@"D:\Projects\BarBlueprintEditor\tmp\commonSchema.json",

                @"csharp",
                sourceFilePathOption.Name,
                @"D:\Projects\BarBlueprintEditor\tmp\commonSchema.json",
                targetFolderOption.Name,
                @"D:\Projects\BarBlueprintEditor\tmp\csharp.cs",
            ];
        }

        var rootCommand = new RootCommand();

        var ddsCommand = new Command("dds", "Convert dds files to png");
        ddsCommand.Options.Add(sourceFolderOption);
        ddsCommand.Options.Add(targetFolderOption);
        ddsCommand.Options.Add(unitDefinitionsFilePathOption);
        ddsCommand.SetAction(parseResult =>
        {
            var sourceFolder = parseResult.GetValue(sourceFolderOption);
            var targetFolder = parseResult.GetValue(targetFolderOption);
            var unitDefinitionsFilePath = parseResult.GetValue(unitDefinitionsFilePathOption);

            var unitInfos = JsonSerializer.Deserialize<List<WebUnitDefinition>>(File.ReadAllText(unitDefinitionsFilePath));
            var unitInfosMap = unitInfos.ToDictionary(x => x.Name);
            var converter = new DdsToImageConverter(unitInfosMap);
            var skipRegex = new Regex("scavengers", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            FileWalker.Walk(sourceFolder, targetFolder, converter, skipRegex);
        });
        rootCommand.Subcommands.Add(ddsCommand);

        var luaCommand = new Command("lua", "Convert lua files to json");
        luaCommand.Options.Add(sourceFolderOption);
        luaCommand.Options.Add(targetFolderOption);
        luaCommand.Options.Add(unitDefinitionsFilePathOption);
        luaCommand.SetAction(parseResult =>
        {
            var sourceFolder = parseResult.GetValue(sourceFolderOption);
            var targetFolder = parseResult.GetValue(targetFolderOption);
            var unitInfos =
                JsonSerializer.Deserialize<List<WebUnitDefinition>>(
                    File.ReadAllText(parseResult.GetValue(unitDefinitionsFilePathOption)));
            var unitInfosMap = unitInfos.ToDictionary(x => x.Name);

            var converter = new LuaToJsonConverter(unitInfosMap);
            var skipRegex = new Regex("scavengers", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            FileWalker.Walk(sourceFolder, targetFolder, converter, skipRegex);
        });
        rootCommand.Subcommands.Add(luaCommand);

        var jsonSchemaCommand = new Command("schema", "Creates json schema file from all lua json files");
        jsonSchemaCommand.Options.Add(sourceFolderOption);
        jsonSchemaCommand.Options.Add(targetFilePathOption);
        jsonSchemaCommand.SetAction(async parseResult =>
        {
            var sourceFolder = parseResult.GetValue(sourceFolderOption);
            var targetFilePath = parseResult.GetValue(targetFilePathOption);

            var json = JsonSchemaMerger.GenerateFromFolder(sourceFolder, SearchOption.AllDirectories, "BarUnit");
            await File.WriteAllTextAsync(targetFilePath, json);
        });
        rootCommand.Subcommands.Add(jsonSchemaCommand);

        var webCommand = new Command("web", "Fetches unit definition data from BAR website to json file");
        webCommand.Options.Add(targetFilePathOption);
        webCommand.SetAction(async parseResult =>
        {
            var targetFilePath = parseResult.GetValue(targetFilePathOption);
            var unitInfos = await UnitInfosWebScraper.GetUnitDefinitions();
            var json = JsonSerializer.Serialize(unitInfos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(targetFilePath, json);
        });
        rootCommand.Subcommands.Add(webCommand);

        var csharpCommand = new Command("csharp", "Creates C# classes for json schema");
        csharpCommand.Options.Add(sourceFilePathOption);
        csharpCommand.Options.Add(targetFilePathOption);
        csharpCommand.SetAction(async parseResult =>
        {
            var sourceFilePath= parseResult.GetValue(sourceFilePathOption);
            var targetFilePath = parseResult.GetValue(targetFilePathOption);

            await JsonSchemaFileToClassConverter.Convert(sourceFilePath, targetFilePath, "BarBlueprintEditor.Shared.Dtos");
        });
        rootCommand.Subcommands.Add(csharpCommand);

        return await rootCommand.Parse(args).InvokeAsync();
    }
}