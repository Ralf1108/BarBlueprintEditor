using System.Text.RegularExpressions;
using BarBlueprintEditor.Shared.Dtos;
using LuaToJsonConverter.Converters;

namespace LuaToJsonConverter;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length  == 0)
        {
            args =
            [
                @"lua",
                @"D:\Projects\BarBlueprintEditor\tmp\git\units",

                //@"dds",
                //@"D:\Projects\BarBlueprintEditor\tmp\git\unitpics",

                @"D:\Projects\BarBlueprintEditor\tmp\converted"
            ];
        }

        if (args.Length != 3)
        {
            Console.WriteLine("Usage: LuaToJsonConverter type <sourceFolder> <targetFolder>");
            return 1;
        }

        var type = args[0];
        var sourceFolder = args[1];
        var targetFolder = args[2];

        Dictionary<string, WebUnitDefinition> unitInfos;
        try
        {
            unitInfos = (await UnitInfosWebScraper.GetUnitDefinitions()).ToDictionary(x => x.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching unit infos from BAR homepage: {ex.Message}");
            return 2;
        }

        var skipRegex = new Regex("scavengers", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        try
        {
            IFileConverter? converter;
            switch (type)
            {
                case "lua":
                   
                    converter = new Converters.LuaToJsonConverter(unitInfos);
                    break;
                case "dds":
                    converter = new DdsToImageConverter(unitInfos);
                    break;
                default:
                    throw new InvalidOperationException("Unknown converter for type: " + type);
            }

            FileWalker.Walk(sourceFolder, targetFolder, converter, skipRegex);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting lua to json: {ex.Message}");
            return 3;
        }
    }
}