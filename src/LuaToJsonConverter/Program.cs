using BarBlueprintEditor.Shared.Dtos;

namespace LuaToJsonConverter;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            args =
            [
                @"D:\Projects\BarBlueprintEditor\tmp\git\units",
                @"D:\Projects\BarBlueprintEditor\tmp\converted"
            ];
        }

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: LuaToJsonConverter <sourceFolder> <targetFolder>");
            return 1;
        }

        Dictionary<string, WebUnitDefinition> unitInfos;
        try
        {
            unitInfos = (await ImageUrlExtractor.GetUnitDefinitions()).ToDictionary(x => x.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching unit infos from BAR homepage: {ex.Message}");
            return 2;
        }

        var sourceFolder = args[0];
        var targetFolder = args[1];
        try
        {
            Converter.Convert(sourceFolder, targetFolder, unitInfos);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting lua to json: {ex.Message}");
            return 3;
        }
    }
}