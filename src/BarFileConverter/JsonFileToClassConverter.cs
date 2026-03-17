using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using System.Text.RegularExpressions;

namespace BarFileConverter;

/// <summary>
/// Generates C# classes which contains all json properties from all BAR json files
/// </summary>
public static class JsonSchemaFileToClassConverter
{
    public static async Task Convert(
        string schemaFilePath,
        string outputFilePath,
        string namespaceName = "Common")
    {
        var schemaJson = await File.ReadAllTextAsync(schemaFilePath);
        var schema = await JsonSchema.FromJsonAsync(schemaJson);

        var allNames = new HashSet<string>();
        var settings = new CSharpGeneratorSettings
        {
            JsonLibrary = CSharpJsonLibrary.SystemTextJson,
            
            ClassStyle = CSharpClassStyle.Poco,
            Namespace = namespaceName,

            UseRequiredKeyword = true,
            GenerateOptionalPropertiesAsNullable = true,

            DateType = "System.DateTime",
            DateTimeType = "System.DateTimeOffset",
            TimeType = "System.TimeSpan",

            PropertyNameGenerator = new BarPropertyNameGenerator(allNames),
            TypeNameGenerator = new BarTypeNameGenerator(allNames),
        };
        var generator = new CSharpGenerator(schema, settings);

        var file = generator.GenerateFile();

        //var outFolder = @"D:\Projects\BarBlueprintEditor\src\BarBlueprintEditor.Shared\Dtos";
        await File.WriteAllTextAsync(outputFilePath, file);
        //await File.WriteAllTextAsync(Path.Combine(outFolder, "Names.txt"), string.Join(Environment.NewLine, allNames.Order()));

        /* Prompt:
        try to fix all names to conform to Pascal casing.
        The result should be a list with one line per source word with a mapping, e.g.
        Armbeamerweapon,ArmbeamerWeapon
        Weapontimer,WeaponTimer

        Here is the source list:
        */
    }
}

public class BarTypeNameGenerator(HashSet<string> allNames) : ITypeNameGenerator
{
    private DefaultTypeNameGenerator defGenerator = new();

    public string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
    {
        var name = defGenerator.Generate(schema, typeNameHint, reservedTypeNames);
        if (name == "Anonymous")
        {
            return "LuaUnit2";
        }

        var result = NameHelper.ToPascalCase(name); // e.g. Smart_trajectory_dummy
        result = NameHelper.ToValidName(result); // e.g. 10
        allNames.Add(result);
        return result;
    }
}

public class BarPropertyNameGenerator(HashSet<string> allNames) : IPropertyNameGenerator
{
    public string Generate(JsonSchemaProperty property)
    {
        var name = property.Name;
        var result = NameHelper.ToPascalCase(name); // e.g. double Edge
        result = NameHelper.ToValidName(result); // e.g. 10
        allNames.Add(result);
        return result;
    }
}

public class NameHelper
{
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // 1. Replace all _ or - with space + next char will be uppercased
        // 2. Make first letter uppercase too
        var result = Regex.Replace(input, @"[-_]+([a-zA-Z])", m => m.Groups[1].Value.ToUpper());

        // If first char is lowercase → uppercase it
        if (char.IsLower(result[0]))
        {
            result = char.ToUpper(result[0]) + result.Substring(1);
        }

        return result;
    }

    public static string ToValidName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (char.IsDigit(input[0]))
            return "_" + input;

        return input;
    }
}
