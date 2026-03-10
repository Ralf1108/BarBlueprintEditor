namespace LuaToJsonConverter;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            args =
            [
                @"D:\Projects\BarBlueprintEditor\tmp\git\units",
                @"D:\Projects\BarBlueprintEditor\tmp\converted"
            ];

            args =
            [
                @"git\units",
                @"converted"
            ];
        }

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: LuaToJsonConverter <sourceFolder> <targetFolder>");
            return 1;
        }

        var sourceFolder = args[0];
        var targetFolder = args[1];
        try
        {
            Converter.Convert(sourceFolder, targetFolder);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting lua to json: {ex.Message}");
            return 2;
        }
    }
}